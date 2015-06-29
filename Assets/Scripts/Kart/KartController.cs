using UnityEngine;
using System.Collections.Generic;

public class KartController : MonoBehaviour 
{
    [SerializeField]
    private float motorTorque;
    [SerializeField]
    private float brakeTorque;
    [SerializeField]
    private float topSpeed;
    [SerializeField]
    private float steerAngle;
    [SerializeField]
    private Vector3 centerOfMass;
    [SerializeField]
    private List<KartWheel> wheels;

    internal float inputHorizontalValue;
    internal float inputVerticalValue;
    internal bool isInputVerticalPressed;

    private float averageWheelSpeed;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        inputHorizontalValue = 0;
        inputVerticalValue = 0;
        isInputVerticalPressed = false;

        averageWheelSpeed = 0;
    }

    void Update()
    {
        foreach (KartWheel wheel in wheels)
        {
            wheel.UpdateTransform();
        }
    }

    void FixedUpdate()
    {
        foreach (KartWheel wheel in wheels)
        {
            wheel.UpdateSpeed();
            wheel.ApplyMotorTorque(motorTorque * inputVerticalValue, topSpeed);
            wheel.ApplySteering(steerAngle * inputHorizontalValue);
            wheel.ApplyBrakeTorque(brakeTorque, inputVerticalValue, isInputVerticalPressed);
        }

        UpdateAverageWheelSpeed();
    }

    private void UpdateAverageWheelSpeed()
    {
        averageWheelSpeed = 0;
        int motorWheelCount = 0;
        foreach (KartWheel wheel in wheels)
        {
            if (wheel.hasMotor)
            {
                averageWheelSpeed += wheel.currentSpeed;
                motorWheelCount++;
            }
        }
        averageWheelSpeed /= motorWheelCount;
    }

    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        if (!GetComponent<NetworkView>().isMine)
        {
            if (GetComponent<KartShoot>() != null)
                Destroy(GetComponent<KartShoot>());
            if (GetComponent<KartGun>() != null)
                Destroy(GetComponent<KartGun>());
            if (GetComponent<KartCamera>() != null)
            {
                GetComponent<KartCamera>().GetCamera().GetComponent<AudioListener>().enabled = false;
                GetComponent<KartCamera>().GetCamera().enabled = false;
                Destroy(GetComponent<KartCamera>());
            }
            if (GetComponent<Skill>() != null)
            {
                foreach (Skill skill in GetComponents<Skill>())
                    Destroy(skill);
            }
            if (GetComponent<InputManager>() != null)
                Destroy(GetComponent<InputManager>());
            Destroy(this);
        }
    }
}

[System.Serializable]
public class KartWheel
{
    [SerializeField]
    internal WheelCollider wheelCollider;
    [SerializeField]
    internal Transform wheelTransform;
    [SerializeField]
    internal bool canSteer;
    [SerializeField]
    internal bool hasMotor;

    internal float currentMotorTorque = 0;
    internal float currentSpeed = 0;
    internal float currentSteerAngle = 0;

    internal void UpdateSpeed()
    {
        currentSpeed = 2f * Mathf.PI * wheelCollider.radius * wheelCollider.rpm * 60f / 1000f;
    }

    internal void ApplyMotorTorque(float motorTorque, float topSpeed)
    {
        if (hasMotor)
        {
            if (currentSpeed < topSpeed && currentSpeed > -topSpeed)
            {
                currentMotorTorque = motorTorque;
            }
            else
            {
                currentMotorTorque = 0;
            }
            wheelCollider.motorTorque = currentMotorTorque;
        }
    }

    internal void ApplyBrakeTorque(float brakeTorque, float inputVerticalValue, bool isInputVerticalPressed)
    {
        if (hasMotor)
        {
            if (isInputVerticalPressed)
            {
                if (currentSpeed > 0)
                {
                    if (inputVerticalValue > 0)
                        wheelCollider.brakeTorque = 0;
                    else
                        wheelCollider.brakeTorque = brakeTorque;
                }
                else if (currentSpeed < 0)
                {
                    if (inputVerticalValue < 0)
                        wheelCollider.brakeTorque = 0;
                    else
                        wheelCollider.brakeTorque = brakeTorque;
                }
                else
                    wheelCollider.brakeTorque = 0;
            }
            else
            {
                wheelCollider.brakeTorque = brakeTorque;
            }
        }
    }

    internal float ApplySteering(float steerAngle)
    {
        if (canSteer)
        {
            currentSteerAngle = steerAngle;
            wheelCollider.steerAngle = currentSteerAngle;
        }
        return currentSteerAngle;
    }

    internal void UpdateTransform()
    {
        wheelTransform.Rotate(new Vector3(wheelCollider.rpm / 60 * 360 * Time.deltaTime, 0, 0));
        wheelTransform.localEulerAngles = new Vector3(wheelTransform.localEulerAngles.x, wheelCollider.steerAngle - wheelTransform.localEulerAngles.z, wheelTransform.localEulerAngles.z);

        RaycastHit hitInfo;
        if (Physics.Raycast(
            wheelCollider.transform.position + wheelCollider.center, 
            -wheelCollider.transform.up, 
            out hitInfo, 
            wheelCollider.radius + wheelCollider.suspensionDistance))
        {
            wheelTransform.position = hitInfo.point - wheelCollider.center + wheelCollider.transform.up * wheelCollider.radius;
        }
    }
}
