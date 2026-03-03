using UnityEngine;
using System.Collections.Generic;

public class MeshGroup : MonoBehaviour
{
    [SerializeField] MeshRenderer[] meshList;

    private void Start()
    {
        meshList = GetComponentsInChildren<MeshRenderer>();
    }

    public void EnableAllMesh()
    {
        foreach (MeshRenderer renderer in meshList)
        {
            renderer.enabled = true;
        }
    }

    public void DisableAllMesh()
    {
        Debug.Log("Called Disable mesh" + this.gameObject.name, this);

        foreach (MeshRenderer renderer in meshList)
        {
            renderer.enabled = false;
            Debug.Log("Disable mesh" + this.gameObject.name, this);
        }
    }
}
