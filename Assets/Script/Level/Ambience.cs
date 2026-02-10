using UnityEngine;

public class Ambience : MonoBehaviour
{
    [SerializeField] Collider areaCollider;
    GameObject _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("PlayerCollider");
    }

    private void Update()
    {
        Vector3 _closestPoint = areaCollider.ClosestPoint(_player.transform.position);
        transform.position = _closestPoint;
    }
}
