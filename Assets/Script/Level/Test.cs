using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

public class Test : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Wait(5));
    }

    private IEnumerator<WaitForSeconds> Wait(float seconds)
    {
        Debug.Log("waiting");
        yield return new WaitForSeconds(seconds);
        Debug.Log("wait end");
    }
}
