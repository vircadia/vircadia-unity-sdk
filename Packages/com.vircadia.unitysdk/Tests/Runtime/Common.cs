//
//  Common.cs
//  Tests/Runtime
//
//  Created by Nshan G. on 18 Feb 2022.
//  Copyright 2022 Vircadia contributors.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using System.IO;

public static class Log
{
    public static void WriteLine(string line)
    {
        Console.WriteLine("[VIRCADIA_SDK_TEST_LOG] " + line);
    }
}
