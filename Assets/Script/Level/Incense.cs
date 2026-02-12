using NUnit.Framework;
using UnityEngine;

public class Incense : MonoBehaviour
{
    [SerializeField] GameObject startObject;
    [SerializeField] GameObject endObject;
    [SerializeField] GameObject incenseStick;
    [SerializeField] Light incenseLight;

    [SerializeField] Material incenseMaterial;
    [SerializeField] Color emissionColorValue;
    public float intensity;

    float startingLight;
    private Vector3 initialScale;
    private Vector3 initialDistance;
    private float distance;
    public float incensePercentage;

    private void OnMouseOver()
    {
        
    }
    private void OnMouseExit()
    {
        
    }

    private void Start()
    {
        SetUpIncense();
        startingLight = incenseLight.range;
        emissionColorValue = incenseMaterial.color;
    }

    private void Update()
    {
        UpdateTransformForScale();
        intensity = (incensePercentage * 4) - 2.5f;
        incenseMaterial.SetColor("_EmissionColor", emissionColorValue * Mathf.Pow(2, intensity));
        incenseLight.intensity = incensePercentage * 0.03f;
    }

    private void SetUpIncense()
    {
        initialScale = incenseStick.transform.localScale;
        initialDistance = (endObject.transform.position - startObject.transform.position);
        endObject.transform.position = startObject.transform.position + (initialDistance * incensePercentage);
    }

    private void UpdateTransformForScale()
    {
        if(incensePercentage >= 0)
        {
            endObject.transform.position = startObject.transform.position + (initialDistance * incensePercentage);
        }

        distance = Vector3.Distance(startObject.transform.position, endObject.transform.position);
        incenseStick.transform.localScale = new Vector3(initialScale.x, distance / 2, initialScale.z);

        Vector3 middlePoint = (startObject.transform.position + endObject.transform.position) /  2;
        incenseStick.transform.position = middlePoint;

        incenseLight.range = startingLight * incensePercentage;

    }

}
