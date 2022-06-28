//
//  Common.cs
//  Runtime/Scripts/Vircadia/Native
//
//  Created by Nshan G. on 18 Feb 2022.
//  Copyright 2022 Vircadia contributors.
//  Copyright 2022 DigiSomni LLC.
//
//  Distributed under the Apache License, Version 2.0.
//  See the accompanying file LICENSE or http://www.apache.org/licenses/LICENSE-2.0.html
//

namespace VircadiaNative
{
    public static class DLLConstants {
      #if UNITY_IOS || UNITY_TVOS
      public const string Import = "__Internal";
      #else
      public const string Import = "vircadia-client";
      #endif
    }

    public static class DataConstants {
        public const int RFC4122ByteSize = 16;
    }
}
