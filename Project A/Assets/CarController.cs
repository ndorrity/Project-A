using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Wheel Meshes")]
    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform rearLeftTransform;
    public Transform rearRightTransform;

    [Header("Car Settings")]
    public float maxMotorTorque = 1500f;
    public float maxSteerAngle = 30f;
    public float brakeForce = 3000f;

    private void Start()
    {
        // Lower center of mass for stability (optional)
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    private void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steer = maxSteerAngle * Input.GetAxis("Horizontal");
        bool isBraking = Input.GetKey(KeyCode.Space);
        float currentBrake = isBraking ? brakeForce : 0f;

        // Motor torque (usually on rear wheels for RWD car)
        rearLeftWheel.motorTorque = motor;
        rearRightWheel.motorTorque = motor;

        // Steering
        frontLeftWheel.steerAngle = steer;
        frontRightWheel.steerAngle = steer;

        // Braking
        frontLeftWheel.brakeTorque = currentBrake;
        frontRightWheel.brakeTorque = currentBrake;
        rearLeftWheel.brakeTorque = currentBrake;
        rearRightWheel.brakeTorque = currentBrake;

        // Visual updates
        UpdateWheelPose(frontLeftWheel, frontLeftTransform);
        UpdateWheelPose(frontRightWheel, frontRightTransform);
        UpdateWheelPose(rearLeftWheel, rearLeftTransform);
        UpdateWheelPose(rearRightWheel, rearRightTransform);
    }

    private void UpdateWheelPose(WheelCollider collider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
}
