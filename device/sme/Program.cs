using System;
using System.Linq;
using SME;

namespace ByteSwap
{
    class MainClass
    {
        public static void Main(string[] args)
        {

            using(var sim = new Simulation())
            {
                uint[] inputdata = {
                    1, 2, 4, 8, 16, 32,
                    64, 128, 256, 1024, 2048, 4096,
                };
                var proc = new ByteSwapProc();
                var simulator = new ValueSimulator(proc, inputdata);

                // Use fluent syntax to configure the simulator.
                // The order does not matter, but `Run()` must be 
                // the last method called.

                // The top-level input and outputs are exposed
                // for interfacing with other VHDL code or board pins

                sim
                        .AddTopLevelOutputs(proc.Output)
                        .AddTopLevelInputs(proc.Input)
                        .BuildCSVFile()
                        .BuildVHDL()
                .Run();

                // After `Run()` has been invoked the folder
                // `output/vhdl` contains a Makefile that can
                // be used for testing the generated design
            }
        }
    }
}
