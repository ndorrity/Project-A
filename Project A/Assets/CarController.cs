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
    public float maxMotorTorque = 1600f;
    public float maxSteerAngle = 32f;
    public float brakeForce = 2500f;

    [Header("Drift Settings")]
    [Tooltip("Sideways stiffness during normal driving")]
    public float normalSidewaysStiffness = 1.0f;
    [Tooltip("Lower stiffess allows the rear to slide sideways")]
    public float driftSidewaysStiffness = 0.38f;
    [Tooltip("Multiplier applied to steering angle while holding handbrake
    public float driftSteerMultiplier = 1.25f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.6f, 0.2f);
    }

    private void FixedUpdate()
    {
        float verticalInput = 0f;
        float horizontalInput = 0f;
        bool isDrifting = false;

        #if ENABLE_INPUT_SYSTEM
        UnityEngine.InputSystem.Keyboard kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) verticalInput += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) verticalInput -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontalInput += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontalInput -= 1f;
            isDrifting = kb.spaceKey.isPressed;
        }
        #else
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        isDrifting = Input.GetKey(KeyCode.Space);
        #endif

        float motor = maxMotorTorque * verticalInput;
        float steer = maxSteerAngle * horizontalInput * (isDrifting ? driftSteerMultiplier : 1.0f);

        frontLeftWheel.steerAngle = steer;
        frontRightWheel.steerAngle = steer;

        rearLeftWheel.motorTorque = motor;
        rearRightWheel.motorTorque = motor;

        if (isDrifting)
        {
            frontLeftWheel.brakeTorque = 0f;
            frontRightWheel.brakeTorque = 0f;

            rearLeftWheel.brakeTorque = brakeForce * 0.45f;
            rearRightWheel.brakeTorque = brakeForce * 0.45f;

            SetWheelSidewaysStiffness(rearLeftWheel, driftSidewaysStiffness);
            SetWheelSidewaysStiffness(rearRightWheel, driftSidewaysStiffness);
        }
        else
        {
            bool isFootBraking = (verticalInput < 0 && rb.linearVelocity.magnitude > 1f && Vector3.Dot(transform.forward, rb.linearVelocity) > 0.5f);
            float appliedBrake = isFootBraking ? brakeForce : 0f;

            frontLeftWheel.brakeTorque = appliedBrake;
            frontRightWheel.brakeTorque = appliedBrake;
            rearLeftWheel.brakeTorque = appliedBrake;
            rearRightWheel.brakeTorque = appliedBrake;

            SetWheelSidewaysStiffness(rearLeftWheel, normalSidewaysStiffness);
            SetWheelSidewaysStiffness(rearRightWheel, normalSidewaysStiffness);
        }

        UpdateWheelPose(frontLeftWheel, frontLeftTransform);
        UpdateWheelPose(frontRightWheel, frontRightTransform);
        UpdateWheelPose(rearLeftWheel, rearLeftTransform);
        UpdateWheelPose(rearRightWheel, rearRightTransform);
    }

    private void SetWheelSidewaysStiffness(WheelCollider collider, float stiffness)
    {
        WheelFrictionCurve curve = collider.sidewaysFriction;
        curve.stiffness = stiffness;
        collider.sidewaysFriction = curve;
    }

    private void UpdateWheelPose(WheelCollider collider, Transform wheelTransform)
    {
        if (collider == null || wheelTransform == null) return;
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
}
