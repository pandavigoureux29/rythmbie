using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameManager m_gameManager;
    
    private GameplayActions m_gameplayActions;

    void Awake()
    {
        m_gameplayActions = new GameplayActions();
        
        m_gameplayActions.BattleController.LeftTap.Enable();
        m_gameplayActions.BattleController.LeftTap.started += context => StartLeftTap(context, InputRegion.LEFT);
        
        m_gameplayActions.BattleController.RightTap.Enable();
        m_gameplayActions.BattleController.RightTap.started += context => StartLeftTap(context, InputRegion.RIGHT);
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
}
