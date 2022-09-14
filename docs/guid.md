### Guid Notes

C# Guid class has endianness dependent internal representation of the 16 byte UUID, and its string representation will not exactly correspond to the other implementations (native client, Web SDK or the server) - the first 8 bytes will be rearranged. For communication with the server the SDK uses the binary representation, that can be retrieved with the `toByteArray` method or set with byte array constructor and is the same across all implementations.
