using System;

public class InputSystem : Singleton<InputSystem>
{
    public InputMaster Input;
    public event Action UseAction;
    
    private void Awake()
    {
        Input = new InputMaster();
        Input.Player.Action.performed += context => UseAction?.Invoke();
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