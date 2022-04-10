using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    
    public float rotationTime = 0;

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
    }
    private AimingState aimState; // controls the aiming for the cannon

    // The following 2 fields are used to determine in a button press has been made
    private float currentInput;
    private float previousInput;
    
    // force to apply to the projectile on shoot.
    // calculated in CalculateForce
    private Vector3 calculatedForce; // this could be a property...

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
                ProcessInput();
                break;
            case State.GetTargetPosition:
                UpdateTargetPosition();
                break;
            case State.CalculateForce:
                CalculateForce();
                break;
            case State.ApplyRotation:
                RotateToTarget();
                break;
            case State.Shoot:
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
    }

    void RotateToTarget()
    {
        // TODO - Create a quaternion using the MathPhysicsEngine, then rotate to the desired rotation using Unity. 
        // (May also try the SLERP technique to smoothly transition to new orientation)
        switch(aimState)
        {
            case AimingState.None:
                InitiateAiming();
                break;
            case AimingState.FindRotation:
                // Create a Quaternion from Euler Angles which represent the force
                break;
            case AimingState.Rotating:
                // SLERP
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
}
