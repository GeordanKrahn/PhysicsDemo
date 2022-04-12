using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static System.Math;
using static UnityEngine.Mathf;

public class CannonController : MonoBehaviour
{
    // The following 4 properties and fields
    // control the interpolator.
    public float maxRotationTime;
    public int numInterpolations;
    public float timeStep
    {
        get
        {
            return maxRotationTime / (float)numInterpolations;
        }
    }
    
    float rotationTime = 0;

    // the following 4 properties and fields keep track of the projectile positions.
    public List<Transform> ListOfTargets;
    public GameObject projectile;
    private Tuple<Vector3, bool> NextTarget
    {
        get
        {
            Vector3 position = Vector3.zero;
            bool hasTarget = false;
            if(ListOfTargets.Count > 0)
            {
                position = ListOfTargets[0].position;
                ListOfTargets.RemoveAt(0);
                hasTarget = true;
            }
            if(!hasTarget)
            {
                Debug.LogWarning("List of targets is empty");
            }
            return new Tuple<Vector3, bool>(position, hasTarget);
        }
    }
    private Vector3 currentTargetPosition;
    private Quaternion TargetOrientation;
    private Quaternion CurrentOrientation;
    double angleBetweenQuaternions;

    // The following enum types control the functionality of the cannon based on current state.
        // The State enum type applies to the stage in cannon operation
        // The AimingState essentially controls the rotation of the cannon
    private enum State
    {
        WaitForTarget,
        GetTargetPosition,
        CalculateForce,
        ApplyRotation,
        Shoot
    }
    private State state; // controls cannons current state

    private enum AimingState
    {
        None,
        FindRotation,
        Rotating,
        Finished
    }
    private AimingState aimState; // controls the aiming for the cannon

    // The following 2 fields are used to determine in a button press has been made
    private float currentInput;
    private float previousInput;
    
    // force to apply to the projectile on shoot.
    // calculated in CalculateForce
    private Vector3 calculatedForce; // this could be a property...
    Vector3 directionVector;

    void Start()
    {
        state = State.WaitForTarget;
        aimState = AimingState.None;
        currentInput = 0;
    }


    void FixedUpdate()
    {
        currentInput = Input.GetAxis("Submit");
        switch(state)
        {
            case State.WaitForTarget:
                Debug.Log("Wait For Target");
                ProcessInput();
                break;
            case State.GetTargetPosition:
                Debug.Log("Get Position");
                UpdateTargetPosition();
                break;
            case State.CalculateForce:
                Debug.Log("Get Force");
                CalculateForce();
                break;
            case State.ApplyRotation:
                Debug.Log("Apply Rotation");
                RotateToTarget();
                break;
            case State.Shoot:
                Debug.Log("Shoot");
                Shoot();
                break;
            default:
                Debug.LogError($"Unknown or invalid state\n{state}");
                break;
        }
        previousInput = currentInput;
    }

    void ProcessInput()
    {
        if(currentInput == 0 && previousInput > 0)
        {
            state = State.GetTargetPosition;
        }
    }

    void UpdateTargetPosition()
    {
        var newTarget = NextTarget;
        if(newTarget.Item2)
        {
            currentTargetPosition = newTarget.Item1;
            state = State.CalculateForce;
        }
        else
        {
            state = State.WaitForTarget;
        }
    }

    void CalculateForce()
    {
        // TODO - use the MathPhysics Engine to calculate the necessary force to apply to the projectile
        // 1: Determine Velocity required for trajectory
        var deltaPosition = currentTargetPosition - transform.position;
        float t = 1.0f; // assume 1 second (we will count up to one second for both rotating, and projectile traveling.)
        var gravity = Physics.gravity;
        var finalVelocity = (deltaPosition + 0.5f * gravity * Mathf.Pow(t, 2)) * (1 / t);
        var initialVelocity = finalVelocity - (gravity * t);
        directionVector = initialVelocity;
        calculatedForce = new((float)initialVelocity.x, (float)initialVelocity.y, (float)initialVelocity.z);
        state = State.ApplyRotation;
    }

    void RotateToTarget()
    {
        // (May also try the SLERP technique to smoothly transition to new orientation)
        switch(aimState)
        {
            case AimingState.None:
                Debug.Log("aiming: None");
                InitiateAiming();
                break;
            case AimingState.FindRotation:
                // Create a Quaternion from Euler Angles which represent the force
                Debug.Log("aiming: FindRotation");
                FindRotation();
                break;
            case AimingState.Rotating:
                // SLERP
                Debug.Log("aiming: SLERP");
                SLERP();
                break;
            case AimingState.Finished:
                Debug.Log("aiming: FINISH");
                // Clean up everything and go to the Shoot state
                aimState = AimingState.None;
                rotationTime = 0;
                state = State.Shoot;
                break;
            default:
                break;
        }
    }

    void Shoot()
    {
        // This may need to be improved... perhaps a PrefabPool... ? 
        var newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        newProjectile.GetComponent<Rigidbody>().AddForce(calculatedForce, ForceMode.Impulse);
        state = State.WaitForTarget;
    }

    void InitiateAiming()
    {
        if(state == State.ApplyRotation)
        {
            aimState = AimingState.FindRotation;
        }
    }

    Vector3 ConvertToEulerAngles(Vector3 directionVector)
    {
        return new Vector3(0, Mathf.Atan(directionVector.x/directionVector.z), Mathf.Atan(directionVector.y/directionVector.x));
    }

    void FindRotation()
    {
        var direction = ConvertToEulerAngles(directionVector);
        Quaternion Q = EulerToQuaternion(new Vector3((direction.y), (direction.z), (direction.x))); // create an Euler To Quaternion Method
        TargetOrientation = Q;
        CurrentOrientation = transform.rotation;
        angleBetweenQuaternions = GetAngleBetweenQuaternions(Q);
        aimState = AimingState.Rotating;
    }

    void SLERP()
    {
        Debug.Log($"{rotationTime}");
        if(rotationTime <= maxRotationTime)
        {
            Quaternion Qt = new (
                (float)((((Sin(1 - rotationTime) * (angleBetweenQuaternions))/ Sin(angleBetweenQuaternions)) * CurrentOrientation.x) + ((Sin(angleBetweenQuaternions * rotationTime) / Sin(angleBetweenQuaternions)) * TargetOrientation.x)),
                (float)((((Sin(1 - rotationTime) * (angleBetweenQuaternions))/ Sin(angleBetweenQuaternions)) * CurrentOrientation.y) + ((Sin(angleBetweenQuaternions * rotationTime) / Sin(angleBetweenQuaternions)) * TargetOrientation.y)),
                (float)((((Sin(1 - rotationTime) * (angleBetweenQuaternions))/ Sin(angleBetweenQuaternions)) * CurrentOrientation.z) + ((Sin(angleBetweenQuaternions * rotationTime) / Sin(angleBetweenQuaternions)) * TargetOrientation.z)),
                (float)((((Sin(1 - rotationTime) * (angleBetweenQuaternions))/ Sin(angleBetweenQuaternions)) * CurrentOrientation.w) + ((Sin(angleBetweenQuaternions * rotationTime) / Sin(angleBetweenQuaternions)) * TargetOrientation.w)));
            transform.rotation = Qt;
        }
        else
        {
            FinishRotation();
        }
        rotationTime += timeStep;
    }

    double GetAngleBetweenQuaternions(Quaternion Q)
    {
        var dotProduct = Q.w * CurrentOrientation.w + Q.x * CurrentOrientation.x + Q.y * CurrentOrientation.y + Q.z * CurrentOrientation.z;

        double angle = Mathf.Acos(Q.w * CurrentOrientation.w + Q.x * CurrentOrientation.x + Q.y * CurrentOrientation.y + Q.z * CurrentOrientation.z);
        return angle;
    }

    void FinishRotation()
    {
        aimState = AimingState.Finished;
    }

    Quaternion EulerToQuaternion(Vector3 EulerAngles)
    {
        return new Quaternion(
            ((Cos(EulerAngles.x / 2f) * Sin(EulerAngles.y / 2f)) * Cos(EulerAngles.z / 2f) + Sin(EulerAngles.x / 2f) * Cos(EulerAngles.y / 2f) * Sin(EulerAngles.z / 2f)),
            ((Sin(EulerAngles.x / 2f) * Cos(EulerAngles.y / 2f)) * Cos(EulerAngles.z / 2f) - Cos(EulerAngles.x / 2f) * Sin(EulerAngles.y / 2f) * Sin(EulerAngles.z / 2f)),
            ((Cos(EulerAngles.x / 2f) * Cos(EulerAngles.y / 2f)) * Sin(EulerAngles.z / 2f) - Sin(EulerAngles.x / 2f) * Sin(EulerAngles.y / 2f) * Cos(EulerAngles.z / 2f)),
            ((Cos(EulerAngles.x / 2f) * Cos(EulerAngles.y / 2f)) * Cos(EulerAngles.z / 2f) + Sin(EulerAngles.x / 2f) * Sin(EulerAngles.y / 2f) * Sin(EulerAngles.z / 2f)));
    }
}
