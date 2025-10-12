using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AreaBoundVolumn : MonoBehaviour
{
    public AreaEnum area;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerCollider")
        {
            Debug.Log(other.gameObject.name + " Entered Area : " + area.ToString());
            GameManager.instance.anomalyManager.currentArea = area;
        }
    }
}
