using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector3 force;
    bool currentFire;
    bool previousFire;
    bool currentReset;
    bool previousReset;
    Vector3 initialPosition;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentReset = Input.GetKeyDown(KeyCode.R);
        currentFire = Input.GetKeyDown(KeyCode.F);
        if(!rb.isKinematic)
        {
            Reset();
        }
        else
        {
            ApplyForce();
        }
    }

    void ApplyForce()
    {
        if(!currentFire && previousFire)
        {
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.Impulse);
        }
        previousFire = currentFire;
    }

    void Reset()
    {
        if(!currentReset && previousReset)
        {
            rb.isKinematic = true;
            transform.position = initialPosition;
        }
        previousReset = currentReset;
    }
}
