//
//  Common.cs
//  Scripts/Vircadia/Native
//
//  Created by Nshan G. on 03 Mar 2022.
//  Copyright 2022 Vircadia contributors.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

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
