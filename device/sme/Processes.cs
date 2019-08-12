using System;
using SME;

namespace ByteSwap
{
	/// <summary>
	/// The byte swap process
	/// </summary>
	public class ByteSwapProc : SimpleProcess
	{
		/// <summary>
		/// The bus that we read input value from
		/// </summary>
        	[InputBus]
        	private readonly ValueBus m_input;

		/// <summary>
		/// The bus that we write results to
		/// </summary>
		[OutputBus]
		public ValueBus Output = Scope.CreateBus<ValueBus>();

		/// <summary>
		/// Constructs a new byte swap process
		/// </summary>
		/// <param name="input">The value input bus</param>
		public ByteSwapProc(ValueBus input)
		{
			// The constructor is not translated into hardware,
			// so it is possible to have dynamic and initialization
			// When the simulation "run" method is called,
			// the values of all variables are captured and used for 
			// initialization
            	    	m_input = input ?? throw new ArgumentNullException(nameof(input));
		}

		/// <summary>
		/// The method invoked when all inputs are ready.
		/// The method is only invoked once pr. clock cycle
		/// </summary>
		protected override void OnTick()
		{	
			// write always
			Output.IsValid = false;
			Output.IsReady = true;

			// If the input value is valid, swap the bytes
			if (m_input.IsValid)
			{
				var val = m_input.Value;

				// Send the output
			        Output.Value = val << 16 | val >> 16;
				Output.IsValid = true;
			}
		}
	}
}
