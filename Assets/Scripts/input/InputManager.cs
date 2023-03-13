using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameManager m_gameManager;
    [SerializeField] private int m_maxTouches = 2;
    
    private GameplayActions m_gameplayActions;

    void Awake()
    {
        m_gameplayActions = new GameplayActions();
        
        //CONTROLLER & KEYBOARD
        m_gameplayActions.BattleController.LeftTap.Enable();
        m_gameplayActions.BattleController.LeftTap.started += context => StartLeftTap(context, InputRegion.LEFT);
        
        m_gameplayActions.BattleController.RightTap.Enable();
        m_gameplayActions.BattleController.RightTap.started += context => StartLeftTap(context, InputRegion.RIGHT);
        
        //TOUCH
        m_gameplayActions.BattleTouch.Enable();
        m_gameplayActions.BattleTouch.Press.Enable();
        m_gameplayActions.BattleTouch.Press.performed += context => StartTouchPress(context);
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        m_gameplayActions.Enable();

    }

    private void OnDisable()
    {
        m_gameplayActions.Disable();
    }

    private void Update()
    {
        CheckTouch();
    }

    private void StartLeftTap(InputAction.CallbackContext context, InputRegion inputRegion)
    {
        //TODO maybe use context.starttime for better synchro
        float inputState = context.ReadValue<float>();
        if (inputState > 0)
        {
            m_gameManager.CheckInput(new GameplayInputActionInfos
            {
                inputType = InputActionType.TAP_STARTED,
                inputRegion = inputRegion
            });
        }
        else
        {
            m_gameManager.CheckInput(new GameplayInputActionInfos
            {
                inputType = InputActionType.TAP_RELEASED,
                inputRegion = inputRegion
            });
        }
    }

    private void StartTouchPress(InputAction.CallbackContext context)
    {
        var touch = context.ReadValue<Touch>();
        
        Debug.Log(touch.position);
    }
    
    #region TOUCH_INPUT

    private void CheckTouch()
    {
        var touches = Input.touches.ToList();
        
        #if UNITY_EDITOR
        //simulate a single touch on editor
        if (Input.GetMouseButtonDown(0))
        {
            Touch t = new Touch();
            t.position = Input.mousePosition;
            t.phase = TouchPhase.Began;
            touches.Add(t);
        }else if (Input.GetMouseButtonUp(0))
        {
            
        }
        #endif
        
        if (touches.Count > 0)
        {
            var touchesCount = Mathf.Max(Input.touches.Length-1, m_maxTouches-1);
            for (int i = 0; i < touchesCount; i++)
            {
                var touch = touches[i];
                switch (touch.phase)
                {
                    case TouchPhase.Began :
                        TouchBegan(touch);
                        break;
                }
            }
            
        }
    }

    void TouchBegan(Touch touch)
    {
        var inputRegion = (touch.position.x < Screen.width / 2) ? InputRegion.LEFT : InputRegion.RIGHT;
        m_gameManager.CheckInput(new GameplayInputActionInfos
        {
            inputType = InputActionType.TAP_STARTED,
            inputRegion = inputRegion
        });
    }
    
    #endregion
}
