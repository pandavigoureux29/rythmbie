using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;

namespace LR
{
    public class Pool<T> : IDisposable where T : class, new()
    {
        private readonly Func<T> _factoryMethod;
        private readonly Action<T> _cleanupMethod;
        private readonly Action<T> _disposeMethod;
        private readonly string _itemName;

        private T[] _items = new T[0];
        private float _growthRate = 0.5f;
        private int _freeIndex;

        public int TakenCount => _freeIndex;
        public int FreeCount => _items.Length - _freeIndex;
        public IReadOnlyList<T> FreeItems => new ArraySegment<T>(_items, _freeIndex, FreeCount);

        public static void NullDispose(T item) { }

        public void Dispose()
        {
            ReturnAll();
            Capacity = 0;
        }

        public Pool(Func<T> factoryMethod = null, Action<T> cleanupMethod = null, Action<T> disposeMethod = null)
        {
            _itemName = typeof(T).Name;

            _factoryMethod = factoryMethod;
            _cleanupMethod = cleanupMethod;
            _disposeMethod = disposeMethod;

            if (_disposeMethod == null)
            {
                _disposeMethod = (item =>
                {
                    switch (item)
                    {
                        case Component component:
                            Object.Destroy(component.gameObject);
                            break;
                        case Object unityObject:
                            Object.Destroy(unityObject);
                            break;
                        case IDisposable disposable:
                            disposable.Dispose();
                            break;

                        default:
                            Debug.LogWarning($"Pool Item {_itemName} couldnt be disposed");
                            break;
                    }
                }
                );
            }
        }

        public float GrowthRate
        {
            get => _growthRate;
            set
            {
                if (_growthRate <= 0f)
                    throw new ArgumentException("Growth rate cannot be zero ro negative");
                _growthRate = value;
            }
        }

        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (_freeIndex > value)
                    throw new InvalidOperationException($"Reducing capactity of {_itemName} pool to {value} would make pool lose track of {_freeIndex - value} taken items");
                int oldCapacity = _items?.Length ?? 0;

                for(int i=value; i<oldCapacity; i++)
                {
                    _disposeMethod.Invoke(_items[i]);
                }

                Array.Resize(ref _items, value);

                for (int i = oldCapacity; i < _items.Length; i++)
                    _items[i] = _factoryMethod != null ? _factoryMethod() : new T();
            }
        }

        public T Take()
        {
            if(_freeIndex == _items.Length)
            {
                var oldCapacity = _items.Length;
                Capacity = Mathf.CeilToInt(Math.Max(_items.Length, 1) * (1 + GrowthRate));
                if (oldCapacity == Capacity)
                    throw new InvalidOperationException($"Specified growth rate {GrowthRate} disallows growth for pool of {_itemName}");
            }

            return _items[_freeIndex++];
        }

        public void Return(T item)
        {
            if (_freeIndex <= 0)
                return;

            _freeIndex--;

            _cleanupMethod?.Invoke(item);

            int destinationIndex = _freeIndex;

            int sourceIndex = Array.IndexOf(_items, item, 0, _freeIndex + 1);

            T swappedItem = _items[destinationIndex];
            _items[destinationIndex] = _items[sourceIndex];
            _items[sourceIndex] = swappedItem;
        }

        public void ReturnAll()
        {
            if(_cleanupMethod != null)
            {
                for (int i = 0; i < _freeIndex; i++)
                    _cleanupMethod(_items[i]);
            }

            _freeIndex = 0;
        }
    }
}