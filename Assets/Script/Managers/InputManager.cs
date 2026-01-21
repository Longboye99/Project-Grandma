using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            GameEventsManager.instance.inputEvents.MovePressed(context.ReadValue<Vector2>());
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameEventsManager.instance.inputEvents.StartInteract();
        }
        else if (context.performed)
        {

        }
        else if (context.canceled)
        {
            GameEventsManager.instance.inputEvents.CancelInteract();
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameEventsManager.instance.inputEvents.Pause();
        }
    }
}
