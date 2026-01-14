using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

public class PointClickCameraController : MonoBehaviour
{
    [Header("Camera Movement Setting")]
    public GameObject playerCameraObject;
    [SerializeField] Transform cameraPivot;
    private Camera playerCamera;

    public float sensX;
    public float sensY;

    float currentRotationY;

    float xRotation;
    float yRotation;

    [SerializeField] GameObject flashLight;

    private void Start()
    {
        playerCamera = playerCameraObject.GetComponent<Camera>();
    }


    private void Update()
    {
        FlashLightMovement();

        if(!GetComponent<PointClickCameraMovement>().isTurning)
        {
            MouseCameraMovement();
        }
    }

    private void MouseCameraMovement()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -10, 10);
        yRotation = Mathf.Clamp(yRotation, -10, 10);
        

        cameraPivot.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    private void FlashLightMovement()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition/3);
        RaycastHit hit;
        Vector3 dir;
        if(Physics.Raycast(ray, out hit))
        {
            dir = (hit.point - flashLight.transform.position).normalized;
            flashLight.transform.rotation = Quaternion.LookRotation(dir);
            return;
        }
        else
        {
            dir = (ray.GetPoint(10) - flashLight.transform.position).normalized;
            flashLight.transform.rotation = Quaternion.LookRotation(dir);
        }       
    }

}