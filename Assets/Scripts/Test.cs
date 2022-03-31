using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using static Engine.Mathematics.Functions;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Engine.Mathematics.Eng_Vector3D vector3D = new Engine.Mathematics.Eng_Vector3D();
        Engine.Mathematics.Eng_Quaternion quaternion = new Engine.Mathematics.Eng_Quaternion();
        Debug.Log($"{vector3D} found!");
        Debug.Log($"{quaternion} found!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
