using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 360f; // Degrees per second
    [SerializeField] private Vector3 rotationAxis = Vector3.forward; // Default to rotating around Z axis

    private void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}