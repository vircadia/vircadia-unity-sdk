using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    void Start()
    {
        Debug.Log("Vircadia SDK native API version: " + Vircadia.Info.NativeVersion().full);
    }

}
