using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Engine properties
    public float maxRPM = 7000f;        // Maximum engine RPM
    public float minRPM = 1000f;        // Minimum engine RPM
    public int numGears = 5;            // Number of gears
    public float[] gearRatios;          // Gear ratios for each gear

    private float currentRPM;           // Current engine RPM
    private int currentGear = 1;        // Current gear
    private float engineForce;          // Engine force applied to wheels

    // Wheel references
    public WheelCollider fl_wc, fr_wc, rl_wc, rr_wc; // Individual Wheel Colliders
    private WheelCollider[] wheelColliders;

    // Vehicle handling properties
    public float engineTorque = 2000f;  // Engine torque
    public float maxSpeed = 200f;       // Maximum speed (km/h)

    // Steering properties
    public float maxSteeringAngle = 30f; // Maximum steering angle for front wheels

    // Wheel transforms
    public Transform fl_wheelTransform, fr_wheelTransform, rl_wheelTransform, rr_wheelTransform;

    void Start()
    {
        // Gear ratios: adjust these values for different gear performance
        gearRatios = new float[] { 3.5f, 2.8f, 2.1f, 1.6f, 1.2f, 1.0f };

        // Initialize the array of wheel colliders
        wheelColliders = new WheelCollider[] { fl_wc, fr_wc, rl_wc, rr_wc };
    }

    void Update()
    {
        float speed = GetComponent<Rigidbody>().velocity.magnitude * 3.6f; // Speed in km/h
        Debug.Log("Speed: " + speed + " km/h");

        // Calculate current RPM based on wheel speed
        float averageWheelSpeed = 0f;
        foreach (WheelCollider wheel in wheelColliders)
        {
            averageWheelSpeed += wheel.rpm;
        }
        averageWheelSpeed /= wheelColliders.Length;

        // Assuming the RPM is proportional to wheel speed
        currentRPM = Mathf.Abs(averageWheelSpeed) * gearRatios[currentGear] * 0.5f;
        currentRPM = Mathf.Clamp(currentRPM, minRPM, maxRPM);
        Debug.Log("Current RPM: " + currentRPM);
        
        // Automatic gear shifting
        if (currentGear < numGears - 1 && currentRPM > maxRPM * 0.9f)
        {
            currentGear++;
        }
        else if (currentGear > 1 && currentRPM < minRPM * 1.1f)
        {
            currentGear--;
        }
        
        //Debug.Log("Current Gear: " + currentGear);

        // Handle steering input
        HandleSteering();

        ApplyEngineForce();
        RevLimiter();
        
        // Update wheel positions and rotations
        UpdateWheelPos(fl_wc, fl_wheelTransform);
        UpdateWheelPos(fr_wc, fr_wheelTransform);
        UpdateWheelPos(rl_wc, rl_wheelTransform);
        UpdateWheelPos(rr_wc, rr_wheelTransform);
    }

    void HandleSteering()
    {
        float steeringInput = Input.GetAxis("Horizontal"); // Get steering input
        float steeringAngle = steeringInput * maxSteeringAngle; // Calculate steering angle

        // Set the steering angle for the front wheel colliders
        fl_wc.steerAngle = steeringAngle;
        fr_wc.steerAngle = steeringAngle;

        // Debug output for steering angle
        //Debug.Log("Steering Angle: " + steeringAngle);
    }

    void ApplyEngineForce()
    {
        float throttleInput = Input.GetAxis("Vertical");
        engineForce = throttleInput * engineTorque * gearRatios[currentGear];
        
        //Debug.Log("Engine Force: " + engineForce);

        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.motorTorque = engineForce / wheelColliders.Length;
        }
    }

    void RevLimiter()
    {
        if (currentRPM >= maxRPM)
        {
            // Apply a limiter effect
            currentRPM = maxRPM - 500; // Slightly reduce RPM to simulate limiter
            // Play rev limiter sound here (e.g., audioSource.Play())
        }
    }

    void UpdateWheelPos(WheelCollider col, Transform t)
    {
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);
        
        // Add 90 degrees to the Y-axis
        rot *= Quaternion.Euler(0, 90, 0);
        
        t.position = pos;
        t.rotation = rot;

        // Debug output for wheel position and rotation
        //Debug.Log(t.name + " Position: " + pos + ", Rotation: " + rot.eulerAngles);
    }
}
