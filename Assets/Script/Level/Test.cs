using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

public class Test : MonoBehaviour
{
    public bool highlighted;

    void Update()
    {
        if (highlighted)
        {
            this.GetComponent<Outline>().enabled = true;
        }
        else
        {
            this.GetComponent<Outline>().enabled = false;
        }
    }
}
