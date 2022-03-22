//
//  Info.cs
//  UnityTests
//
//  Created by Nshan G. on 18 Feb 2022.
//  Copyright 2022 Vircadia contributors.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

using System;
using NUnit.Framework;

public class Info
{
    [Test]
    public void VersionPasses()
    {
        var version = Vircadia.Info.NativeVersion();
        Log.WriteLine("Native Client API version is " + version.full);

        Assert.IsTrue(version.year > 0);
        Assert.IsTrue(version.major >= 0);
        Assert.IsTrue(version.minor >= 0);
        Assert.IsFalse(version.major == 0 && version.minor == 0);
        Assert.IsFalse(string.IsNullOrEmpty(version.commit));
        Assert.IsFalse(string.IsNullOrEmpty(version.number));
        Assert.IsFalse(string.IsNullOrEmpty(version.full));
        var number = version.major + "." + version.minor;
        Assert.AreEqual(version.number, number);
        Assert.AreEqual(version.full, "v" + version.year + "." + number + "-git." + version.commit);
    }
}
