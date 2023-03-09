using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteVisual : MonoBehaviour
{
    [SerializeField] private Animator m_animator;

    [SerializeField] private float m_rightRotationY;
    [SerializeField] private float m_leftRotationY;
    
    public void SetDirection(Vector3 direction)
    {
        var rotationY = m_rightRotationY;
        if (direction.x < 0)
            rotationY = m_leftRotationY;
        var eulerAngles = transform.rotation.eulerAngles; 
        eulerAngles.y = rotationY;
        transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
