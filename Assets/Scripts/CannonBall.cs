using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    void OnTriggerEnter(Collider collision)
    {
        LayerMask target = LayerMask.NameToLayer("Target");
        if(collision.gameObject.layer == target)
        {
            Debug.Log("Hit Target");
        }
    }
}
