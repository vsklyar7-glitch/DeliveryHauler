using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickupController : MonoBehaviour
{
    [Header("Настройки автомобиля")]
    public float maxMotorTorque = 1200f;
    public float maxSteeringAngle = 40f;
    public float minSteeringAngle = 12f;
    public float brakeForce = 4000f;

    [Header("Плавность управления")]
    public float accelerationSmoothness = 1.2f;
    public float steeringSmoothness = 5f;

    [Header("Анти-переворот (Стабилизаторы)")]
    [Tooltip("Чем больше значение, тем жестче кузов сопротивляется крену.")]
    public float antiRollForce = 5000f;

    [Header("Центр тяжести")]
    public Transform centerOfMass;

    [Header("Wheel Colliders")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    [Header("Wheel Meshes")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    private float currentSteeringAngle;
    private float targetSteeringAngle;
    private float smoothedVerticalInput;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition;
        }
    }

    private void FixedUpdate()
    {
        HandleSteering();
        HandleMotorAndBraking();
        ApplyAntiRollBars(); // Вызов системы стабилизации
        UpdateWheels();
    }

    private void HandleMotorAndBraking()
    {
        float targetVerticalInput = Input.GetAxis("Vertical");
        smoothedVerticalInput = Mathf.MoveTowards(smoothedVerticalInput, targetVerticalInput, Time.fixedDeltaTime * accelerationSmoothness);

        float forwardSpeed = transform.InverseTransformDirection(rb.linearVelocity).z;
        float currentMotorTorque = 0f;
        float currentBrakeForce = 0f;

        if (Input.GetKey(KeyCode.Space))
        {
            currentBrakeForce = brakeForce;
            smoothedVerticalInput = 0f;
        }
        else
        {
            if (targetVerticalInput > 0)
            {
                if (forwardSpeed < -0.5f) currentBrakeForce = brakeForce;
                else currentMotorTorque = smoothedVerticalInput * maxMotorTorque;
            }
            else if (targetVerticalInput < 0)
            {
                if (forwardSpeed > 0.5f) currentBrakeForce = brakeForce;
                else currentMotorTorque = smoothedVerticalInput * maxMotorTorque;
            }
            else
            {
                smoothedVerticalInput = Mathf.MoveTowards(smoothedVerticalInput, 0f, Time.fixedDeltaTime * accelerationSmoothness * 2f);
                currentBrakeForce = 200f;
            }
        }

        frontLeftCollider.motorTorque = currentMotorTorque;
        frontRightCollider.motorTorque = currentMotorTorque;
        rearLeftCollider.motorTorque = currentMotorTorque;
        rearRightCollider.motorTorque = currentMotorTorque;

        frontLeftCollider.brakeTorque = currentBrakeForce;
        frontRightCollider.brakeTorque = currentBrakeForce;
        rearLeftCollider.brakeTorque = currentBrakeForce;
        rearRightCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float currentSpeedKmH = rb.linearVelocity.magnitude * 3.6f;

        float speedFactor = Mathf.Clamp01(currentSpeedKmH / 80f);
        float dynamicMaxSteerAngle = Mathf.Lerp(maxSteeringAngle, minSteeringAngle, speedFactor);

        targetSteeringAngle = horizontalInput * dynamicMaxSteerAngle;
        currentSteeringAngle = Mathf.MoveTowards(currentSteeringAngle, targetSteeringAngle, Time.fixedDeltaTime * steeringSmoothness * maxSteeringAngle);

        frontLeftCollider.steerAngle = currentSteeringAngle;
        frontRightCollider.steerAngle = currentSteeringAngle;
    }

    // --- МАГИЯ ПРОТИВ ПЕРЕВОРОТОВ ---
    private void ApplyAntiRollBars()
    {
        ApplyAntiRollToAxle(frontLeftCollider, frontRightCollider);
        ApplyAntiRollToAxle(rearLeftCollider, rearRightCollider);
    }

    private void ApplyAntiRollToAxle(WheelCollider left, WheelCollider right)
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = left.GetGroundHit(out hit);
        if (groundedL) travelL = (-left.transform.InverseTransformPoint(hit.point).y - left.radius) / left.suspensionDistance;

        bool groundedR = right.GetGroundHit(out hit);
        if (groundedR) travelR = (-right.transform.InverseTransformPoint(hit.point).y - right.radius) / right.suspensionDistance;

        float antiRollVal = (travelL - travelR) * antiRollForce;

        if (groundedL) rb.AddForceAtPosition(left.transform.up * -antiRollVal, left.transform.position);
        if (groundedR) rb.AddForceAtPosition(right.transform.up * antiRollVal, right.transform.position);
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftCollider, frontLeftMesh);
        UpdateSingleWheel(frontRightCollider, frontRightMesh);
        UpdateSingleWheel(rearLeftCollider, rearLeftMesh);
        UpdateSingleWheel(rearRightCollider, rearRightMesh);
    }

    private void UpdateSingleWheel(WheelCollider collider, Transform meshTransform)
    {
        if (meshTransform == null) return;
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        meshTransform.position = position;
        meshTransform.rotation = rotation;
    }
}