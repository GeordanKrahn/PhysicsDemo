using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CannonController : MonoBehaviour
{
    public List<Transform> ListOfTargets;
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
    private enum State
    {
        WaitForTarget,
        GetTargetPosition,
        CalculateForce,
        ApplyRotation,
        Shoot
    }
    private State state;
    private float currentInput;
    private float previousInput;

    void Start()
    {
        state = State.WaitForTarget;
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
                // TODO
                break;
            case State.CalculateForce:
                // TODO
                break;
            case State.ApplyRotation:
                // TODO
                break;
            case State.Shoot:
                // TODO
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
}
