using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SpriteTurner : MonoBehaviour
{
    GameObject playerCam;

    private void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("Player");
        transform.Rotate(0, 180, 0);

    }

    private void Update()
    {
        TurnSprite();
    }

    private void TurnSprite()
    {
        Vector3 camPosition = playerCam.transform.position;
        camPosition.y = transform.position.y;
        transform.LookAt(camPosition);
        transform.Rotate(0, 180, 0);

    }
}
