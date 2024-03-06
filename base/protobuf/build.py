import os
import glob

# Compiles all proto files to C# and C (using nanopb)

for file in glob.glob("*.proto"):   
    print("Compiling " + file)
    basename = os.path.splitext(file)[0];
    os.system("protoc -o{0}.pb {0}.proto".format(basename))
    print("Generating C# code")
    os.system("protoc {0}.proto --csharp_out=.".format(basename))
    print("Generating C code")
    os.system('nanopb_generator "{0}.pb" "--no-timestamp" "--generated-include-format=#include <planetary/proto/%s>" "--library-include-format=#include <nanopb/pb.h>"'.format(basename))