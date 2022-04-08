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

    void Start()
    {
        state = State.WaitForTarget;
    }


    void FixedUpdate()
    {
        switch(state)
        {
            case State.WaitForTarget:
                // TODO
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
                break;
        }
    }
}
