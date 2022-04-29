### Vircadia Unity SDK

This is a native plugin for Unity 3D engine to support development of client applications for Vircadia Open Source Metaverse Platform. The project itself serves as an example, with the SDK set up as an embedded plugin.

### Getting Started

The SDK package can be imported using the "Add package from tarball..." option in [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html). The package includes additional samples that can be imported for the package manager window. The samples by default expect a server running on localhost, which can be installed following the instructions [here](https://docs.vircadia.com/explore/get-started/install.html).


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
For further details refer to the samples and API documentation.

### Development setup

You'll need to build Vircadia client library that provided a native API for communication with the server side. Clone the main Vircadia [repo](https://github.com/vircadia/vircadia) and follow the instructions for you platform. After the cmake configuration step, instead of building usual components, you can just build the client library by following the instructions in `libraries/vircadia-client` [directory](https://github.com/vircadia/vircadia/tree/unity-sdk/libraries/vircadia-client). Once built copy the client library binary from `build/libraries/vircadia-client` to `Packages/com.vircadia.unitysdk/Runtime/Plugins/` in this project. You might need to copy some dependencies as well, to identify those you can use `ldd` or `objdump -T` or an equivalent. Then you should be able to open the project in Unity Editor and run the unit tests and samples.


The SDK is documentation using standard C# XML formatting. `docs/Doxyfile` is a basic configuration for generating documentation with doxygen:
Running `doxygen ./docs/Doxyfile` in the root of the project will generate html and latex documentation in the `docs` folder.


### Packaging

The native client libraries will need to be built for each supported platform, and copied over to the plugin directory along with all necessary dependencies. The target OS and architecture will need to be configured in the editor for each library file. The `Assets/Samples` directory will need to be copied to `Packages/com.vircadia.unitysdk/` and renamed to `Samples~` to be included in the package.  With that it's sufficient to just create and distribute a tar archive that has a single directory called `package` with the contents of `Packages/com.vircadia.unitysdk` directory. The archive must use a gzip compression and have a .tgz extension. It can be imported directly though Unity Package manager.


This process can be automated once the client library takes shape and it's dependencies on any given platform stabilize.
