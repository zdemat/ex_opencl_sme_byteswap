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
        public readonly ILiteAvalonInput Input = Scope.CreateBus<ILiteAvalonInput>();

		/// <summary>
		/// The bus that we write results to
		/// </summary>
		[OutputBus]
		public readonly ILiteAvalonOutput Output = Scope.CreateBus<ILiteAvalonOutput>();

		private uint m_value;
		private bool m_value_ready = false;

		/// <summary>
		/// The method invoked when all inputs are ready.
		/// The method is only invoked once pr. clock cycle
		/// </summary>
		protected override void OnTick()
		{
			// Check if we are waiting to send a value
			if (m_value_ready)
			{
				// If we are waiting, keep waiting until we have sent it
				if (Input.InputReady)
				{
					// The output has been consumed
					Output.OutputValid = false;
					
					// We can read again
					m_value_ready = false;
					Output.OutputReady = true;
				}
			}
			else
			{
				// Keep waiting until we get a value
				if (Input.InputValid)
				{
					// Output the value
					Output.Value = Input.Value << 16 | Input.Value >> 16;
					// Signal that it can be read
					Output.OutputValid = true;

					// See if we need to block in next cycle
					m_value_ready = !Input.InputReady;
					// We can read again, if this value is consumed
					Output.OutputReady = Input.InputReady;
				}

			}

			// If we sent the value and a new value is ready, consume it immediately
		}
	}
}
