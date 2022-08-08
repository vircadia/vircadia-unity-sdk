### Overview

[Vircadia Unity SDK](https://github.com/vircadia/vircadia-unity-sdk) is a native plugin for Unity 3D engine to support development of client applications for Vircadia Open Source Metaverse Platform. The project itself serves as an example, with the SDK set up as an embedded plugin.

### Getting Started

The SDK package (`com.vircadia.unitysdk.tgz` archive from [project releases on GitHub](https://github.com/vircadia/vircadia-unity-sdk/releases)) can be imported using the "Add package from tarball..." option in [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html). The package includes additional samples that can be imported for the package manager window. The samples and unit tests expect a server running on `localhost` by default, which can be installed following the instructions [here](https://docs.vircadia.com/explore/get-started/install.html).


All components of the SDK are available under `Vircadia` namespace. The main entry point class is the `DomainServer`, which initialized all necessary state, allows connecting to domain servers and provides other components of the SDK as properties.

A basic usage example in pseudocode:
```
    vat domainServer = new Vircadia.DomainServer(); // create an instance
    domainServer.Connect(...); // initiale connection to a server
    while(domainServer.status != Vircadia.DomainServerStatus.Connected) {
        yield return new WaitForSeconds(1); // wait for connection to be established
    }
    domainServer.Messages.Enable(...); // enable the messaging component
    domainServer.Messages.Send(...); // send a message
    domainServer.Destroy(); // disconnect and clean up
```
For further details refer to the code samples and API documentation included in the package. The API documentation for the latest release is also hosted [here]().

### Supported Platforms

The portability of the SDK depends on the [Vircadia Client Library](https://github.com/vircadia/vircadia/tree/unity-sdk/libraries/vircadia-client) that's used as a native plugin. Currently the SDK supports Windows, Linux, MacOS and Android targets, with IOS planned for the future releases (only lacking a build configuration). There are known portability [issues](https://github.com/vircadia/vircadia-unity-sdk/issues/15) with the WebGL target.

### Development setup

To work based on the latest master branch, or other development branches, it might be necessary to setup the Vircadia client library, which is currently part of the main Vircadia [repo](https://github.com/vircadia/vircadia). With CMake properly configured, the client library can be build following the instructions in `libraries/vircadia-client` [directory](https://github.com/vircadia/vircadia/tree/unity-sdk/libraries/vircadia-client). Once built the produced dynamic library should be copied to `Packages/com.vircadia.unitysdk/Runtime/Plugins/` in this project. Some dependencies might need to be copied as well (can be determined using `ldd` or `objdump -p` or an equivalent). Alternatively if the work can be based on the latest release (or if there have been no changes to the native plugin since) it might be sufficient to just copy the `Plugins` folder from the latest release package.


The SDK is documented using standard C# XML formatting. `docs/Doxyfile` is the configuration file for generating documentation with doxygen. There is a `docs` target in the project Makefile that will generate the HTML documentation and add it to the package.

### Packaging

To create a new package the following steps must be taken:
1. Build the native client library for each supported platform, and copy it over to the plugin directory along with all necessary dependencies.
2. Set the target OS and architecture in the Unity editor for each library file (only needs to be done once per project setup, or when new libraries are added).
3. Copy `Assets/Samples` directory to `Packages/com.vircadia.unitysdk/` and renamed it to `Samples~` to be included in the package.
4. Generate html documentation and copy it to `Packages/com.vircadia.unitysdk/Documentation`.
5. Create gzip compressed tar archive archive with `.tgz` extension, that has a single directory called `package` with the contents of `Packages/com.vircadia.unitysdk` directory.

The project Makefile includes a `package` target, which automates steps 3 to 5.
