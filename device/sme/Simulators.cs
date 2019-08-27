using SME;
using System;
using System.Threading.Tasks;

namespace ByteSwap
{
    /// <summary>
    /// Helper process that write a value into the simulation.
    /// Since this is a simulation process, it will not be rendered as hardware
    /// and we can use any code and dynamic properties we want
    /// </summary>
    public class ValueSimulator : SimulationProcess
    {
        [OutputBus]
        private readonly ILiteAvalonInput m_control;
        [OutputBus]
        private readonly ILiteAvalonOutput m_result;
        private readonly uint[] m_values;
        public ValueSimulator(ByteSwapProc proc, uint[] values)
        {
            if (proc == null)
                throw new ArgumentNullException(nameof(proc));

            m_control = proc.Input;
            m_result = proc.Output;
            m_values = values ?? throw new ArgumentNullException(nameof(values));
        }

        /// <summary>
        /// Run this instance.
        /// </summary>
        public override async Task Run()
        {
            // Wait for the initial reset to propagate
            await ClockAsync();

            // Run through all values
            foreach (var val in m_values)
            {
                // We are now transmitting data
                m_control.InputValid = true;
                m_control.Value = val;
                // We can read data
                m_control.InputReady = true;

                Console.WriteLine($"Wrote {val} to swapper");
            
                await ClockAsync();
            
                Console.WriteLine($"Got result: {m_result.Value}");
            }

            // We are now done with the values, so signal that
            m_control.InputValid = false;
            
            // Make sure the last state has propagated
            await ClockAsync();
        }
    }
}
