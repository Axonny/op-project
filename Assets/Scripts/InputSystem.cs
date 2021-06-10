using System;
using System.Collections;

public class InputSystem : Singleton<InputSystem>
{
    public InputMaster Input;
    public event Action UseAction;
    
    private void Awake()
    {
        Input = new InputMaster();
        Input.Player.Action.performed += context => UseAction?.Invoke();
    }

    public void TogglePlayerInput(bool active)
    {
        if (active)
            Input.Player.Enable();
        else
            Input.Player.Disable();
    }

    private void OnEnable()
    {
        Input.Enable();
    }

    private void OnDisable()
    {
        Input.Disable();
    }
}