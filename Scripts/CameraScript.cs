using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform car; // Reference to the car's transform
    public float rotationSpeed = 50f; // Speed of camera rotation
    public float verticalSpeed = 20f; // Speed of vertical movement
    public Vector3 initialOffset; // Initial offset from the car

    private Vector3 currentOffset; // Current offset from the car

    void Start()
    {
        // Set the initial offset from the car's position
        currentOffset = transform.position - car.position;
        initialOffset = currentOffset;
    }

    void Update()
    {
        // Rotate the camera around the car based on input
        if (Input.GetKey(KeyCode.Keypad4))
        {
            transform.RotateAround(car.position, Vector3.up, rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Keypad6))
        {
            transform.RotateAround(car.position, Vector3.up, -rotationSpeed * Time.deltaTime);
        }

        // Move the camera up and down based on input
        if (Input.GetKey(KeyCode.Keypad8))
        {
            transform.position += Vector3.up * verticalSpeed * Time.deltaTime;
            currentOffset = transform.position - car.position;
        }
        if (Input.GetKey(KeyCode.Keypad2))
        {
            transform.position -= Vector3.up * verticalSpeed * Time.deltaTime;
            currentOffset = transform.position - car.position;
        }

        // Reset/center the camera position
        if (Input.GetKey(KeyCode.Keypad5))
        {
            currentOffset = initialOffset;
            transform.position = car.position + currentOffset;
        }

        // Make sure the camera is always looking at the car
        transform.LookAt(car);
    }
}
