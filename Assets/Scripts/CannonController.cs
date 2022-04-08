using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    public List<Transform> ListOfTargets;
    private Vector3 NextTarget
    {
        get
        {
            Vector3 position = ListOfTargets[0].position;
            ListOfTargets.RemoveAt(0);
            return position;
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
