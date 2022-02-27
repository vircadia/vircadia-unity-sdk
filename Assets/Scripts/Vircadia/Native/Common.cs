//
//  Common.cs
//  Scripts/Vircadia/Native
//
//  Created by Nshan G. on 18 Feb 2022.
//  Copyright 2022 Vircadia contributors.
//

namespace VircadiaNative
{
    public static class DLLConstants
    {
      #if UNITY_IOS || UNITY_TVOS
      public const string Import = "__Internal";
      #else
      public const string Import = "vircadia-client";
      #endif
    }
}
