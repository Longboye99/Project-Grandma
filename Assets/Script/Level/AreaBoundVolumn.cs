using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AreaBoundVolumn : MonoBehaviour
{
    public AreaEnum area;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameManager.instance.anomalyManager.currentArea = area;
        }
    }
}
