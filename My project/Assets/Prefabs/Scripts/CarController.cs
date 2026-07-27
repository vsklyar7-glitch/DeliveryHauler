using UnityEngine;

[RequireComponent(typeof(Rigidbody))] 
public class CarController : MonoBehaviour
{
    [Header("")]
    public float motorForce = 1500f;          // ���� ���������
    public float brakeForce = 3000f;          // ���� ������� (������)
    public float activeBrakingForce = 2500f;  // ���� ������� ��� ������� � ��������������� �������
    public float decelerationForce = 300f;    // ���� ���������� (����� ������� ���)
    public float maxSteerAngle = 30f;         // ������������ ���� �������� ����

    [Header("(Wheel Colliders)")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("(Meshes)")]
    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform rearLeftTransform;
    public Transform rearRightTransform;

    private float horizontalInput;
    private float verticalInput;
    private bool isBraking;
    private float currentSteerAngle;

    private Rigidbody rb;

    private void Start()
    {
        // �������� ��������� Rigidbody ��� ������
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotorAndBraking();
        HandleSteering();
        UpdateWheels();
    }

    // �������� ���� �� ������
    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBraking = Input.GetKey(KeyCode.Space);
    }

    // ���������� �����, �������� � �������� �����������
    private void HandleMotorAndBraking()
    {
        // ��������� �������� ������ ������������ � ������������ ����������� (��� Z)
        float forwardSpeed = transform.InverseTransformDirection(rb.linearVelocity).z;

        bool isTryingToBrakeActive = false;

        // ���������, ���� �� ����� � �������, ��������������� ��������
        // (���� ������, �� ���� ����� ��� ���� �����, �� ���� ������)
        // ���������� 1f ��� �����������, ����� ������ �� ��������� �� ������� ��������
        if ((forwardSpeed > 1f && verticalInput < 0) || (forwardSpeed < -1f && verticalInput > 0))
        {
            isTryingToBrakeActive = true;
        }

        // 1. ��������� ���� ���������
        if (isTryingToBrakeActive)
        {
            // ���� �������� ��������� �������� �����������, ��������� �����
            rearLeftWheel.motorTorque = 0;
            rearRightWheel.motorTorque = 0;
        }
        else
        {
            // ����� ���� ��� ������
            float motorTorque = verticalInput * motorForce;
            rearLeftWheel.motorTorque = motorTorque;
            rearRightWheel.motorTorque = motorTorque;
        }

        // 2. ������������ ���� ����������
        float currentBrakeForce = 0f;

        if (isBraking)
        {
            // ������ � ������ ������
            currentBrakeForce = brakeForce;
        }
        else if (isTryingToBrakeActive)
        {
            // �������� ���������� (��� � �������� �������)
            currentBrakeForce = activeBrakingForce;
        }
        else if (verticalInput == 0)
        {
            // ��������� �������
            currentBrakeForce = decelerationForce;
        }

        // 3. ��������� ��������� ������ �� ���� �������
        ApplyBrakingForce(currentBrakeForce);
    }

    // ����� ��� ���������� ���������� �� ���� �������
    private void ApplyBrakingForce(float force)
    {
        frontLeftWheel.brakeTorque = force;
        frontRightWheel.brakeTorque = force;
        rearLeftWheel.brakeTorque = force;
        rearRightWheel.brakeTorque = force;
    }

    // ���������� ���������
    // ���������� ���������
    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheel.steerAngle = currentSteerAngle;
        frontRightWheel.steerAngle = currentSteerAngle;
    }

    // ���������� ����������� �������� � �������� �����
    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheel, frontLeftTransform);
        UpdateSingleWheel(frontRightWheel, frontRightTransform);
        UpdateSingleWheel(rearLeftWheel, rearLeftTransform);
        UpdateSingleWheel(rearRightWheel, rearRightTransform);
    }

    // ����� ��� ������������� ���������� ������ � ��� 3D-�������
    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        if (wheelTransform == null) return;

        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);

        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
}