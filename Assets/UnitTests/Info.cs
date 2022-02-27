//
//  Info.cs
//  UnityTests
//
//  Created by Nshan G. on 18 Feb 2022.
//  Copyright 2022 Vircadia contributors.
//

using System;
using NUnit.Framework;

public class Info
{
    [Test]
    public void BasicSimplePasses()
    {
        var version = Vircadia.Info.NativeVersion();
        Log.WriteLine("Native Client API version is " + version.full);

        Assert.IsTrue(version.major >= 0);
        Assert.IsTrue(version.minor >= 0);
        Assert.IsTrue(version.tweak >= 0);
        Assert.IsFalse(version.tweak == 0 && version.minor == 0 && version.tweak == 0);
        Assert.IsFalse(string.IsNullOrEmpty(version.commit));
        Assert.IsFalse(string.IsNullOrEmpty(version.number));
        Assert.IsFalse(string.IsNullOrEmpty(version.full));
        var number = "v" + version.major +
            "." + version.minor +
            "." + version.tweak;
        Assert.AreEqual(version.number, number);
        Assert.AreEqual(version.full, number + "-git." + version.commit);
    }
}
