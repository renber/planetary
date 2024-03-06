# ![PLANetary logo](https://www.tu-chemnitz.de/~berre/images/planetary_logo_small.png) PLANetary protocol definitions

This repository contains the definition of communication messages for PLANetary in protocol buffer format.
When using PLANetary you can have the message serializaton/deserialization code generated based on in this definitions.

## How to build

In order to compile the proto files you need the protocol buffers compiler [protoc](https://github.com/google/protobuf).

### General building advice
In order to generate structures/classes and seriailization/deserialization code for your target programming language from the proto files, please consult the [protocol buffers manual](https://developers.google.com/protocol-buffers/docs/tutorials).
For example, if you wanted to build C++ classes for the query messages, you would invoke the protoc compiler with
```
protoc -I=$SRC_DIR --cpp_out=$DST_DIR $SRC_DIR/query.proto
```

### Regarding embedded systems
The protocol buffer itself is not optimized for usage in embedded environments (e.g. all integers are 32-bit). Because of this, when generating code for an embedded, memory-restricted system it is recommended to use [nanopb](https://github.com/nanopb/nanopb) which supports additional options for memory optimization. The *.options files in this repository already define appropriate generation restrictions for all message types and can directly be used with nanopb.
To create C structures and code using nanopb, you would compile the .proto files using protoc and the process the resulting .pb file susing nanopb:
```
protoc -oquery.pb query.proto
python nanopb/generator/nanopb_generator.py query.pb
```

You can use the supplied build.py file from this repository to generate C and C# classes of all proto files in one step.