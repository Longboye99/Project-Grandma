using UnityEngine;

public class ToggleInteractable : Interactable
{
    public bool isOpen;
    [SerializeField] Animator animator;

    private void Start()
    {
        useOutline = true;
        if (!isOpen)
        {
            Close();
        }
    }

    public override void Interact(Interactable interactable, AreaEnum area, InteractMode mode)
    {
        if (interactable == this && mode == InteractMode.Click)
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }

    private void Open()
    {
        Debug.Log("Open");
        animator.SetTrigger("Open");
        isOpen = true;
    }

    private void Close()
    {
        Debug.Log("Close");
        animator.SetTrigger("Close");
        isOpen = false;
    }

}
