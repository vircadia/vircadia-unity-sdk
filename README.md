### Vircadia Unity SDK

This is a native native plugin for Unity 3D engine to support development of client applications for Vircadia Open Source Metaverse Platform. The project itself serves as an example, with the SDK set up as an embedded plugin.

### Development setup

You'll need to build Vircadia client library that provided a native API for communication with the server side. Clone the main Vircadia [repo](https://github.com/vircadia/vircadia) and follow the instructions for you platform. After the cmake configuration step, instead of building usual components, you can just build the client library by following the instructions in `libraries/vircadia-client` [directory](). Once build copy the client library binary from `build/libraries/vircadia-client` to `Packages/com.vircadia.unitysdk/Runtime/Plugins/` in this project. You might need to copy some dependencies as well, to identify those you can use `ldd` or `objdump -T` or an equivalent. Then you should be able to open the project in unity editor, open and run the TestScene.


The SDK is documentation using standard C# XML formatting. `docs/Doxyfile` is a basic configuration for generating documentation with doxygen:
Running `doxygen ./docs/Doxyfile` in the root of the project will generate html and latex documentation in the `docs` folder.


### Packaging

The native client libraries will need to be built for each supported platform, and copied over to the plugin directory along with all necessary dependencies. The target OS and architecture will need to be configured in the editor for each library file. With that it's sufficient to just create and distribute a zip/tar archive of the contents of `Packages/com.vircadia.unitysdk` directory. The archive can be imported directly though Unity Package manager.


This process can be automated once the client library takes shape and it's dependencies on any given platform stabilize
