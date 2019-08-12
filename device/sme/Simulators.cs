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
		/// <summary>
		/// The value bus
		/// </summary>
		[OutputBus]
		public readonly ValueBus Data = Scope.CreateBus<ValueBus>();

        	/// <summary>
        	/// The value to process
        	/// </summary>
        	private readonly uint[] VALUES;

        	/// <summary>
        	/// Initializes a new instance of the <see cref="T:ByteSwap.ValueSimulator"/> class.
        	/// </summary>
        	/// <param name="values">The values to process.</param>
        	public ValueSimulator(params uint[] values)
        	{
            		VALUES = values;
        	}

		/// <summary>
		/// Run this instance.
		/// </summary>
		public override async Task Run()
		{
			// Wait for the initial reset to propagate
			await ClockAsync();

			// Run through all values
			foreach (var val in VALUES)
			{
				// We are now transmitting data
				Data.IsValid = true;
				Data.Value = val;
			
				await ClockAsync();
			
				// Write progress
				Console.WriteLine($"value={1} written", val);
			}

			// We are now done with the values, so signal that
			Data.IsValid = false;
			Data.IsReady = true;
			
			// Make sure the last state has propagated
			await ClockAsync();
		}
	}
}
