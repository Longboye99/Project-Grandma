using UnityEngine;

public class ToggleInteractable : Interactable
{
    public bool isOpen;
    [SerializeField] Animator animator;

    private void Start()
    {
        if (!isOpen)
        {
            Close();
        }
    }

    public override void Interact(Interactable interactable, AreaEnum area)
    {
        if (interactable == this)
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
