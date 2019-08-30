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

        /// <summary>
        /// The different states the protocol can be in
        /// </summary>
        private enum ControlStates
        {
            /// <summary>Transaction has completed</summary>
            Init,
            /// <summary>Waiting for input</summary>
            WaitRead,
            ///	<summary>Performing computations</summary>
            Compute,
            ///	<summary>Waiting for output</summary>
            WaitWrite
        }

        /// <summary>
        /// The current state of the process
        /// </summary>
        private ControlStates m_state = ControlStates.Init;

        /// <summary>
        /// The captured input value
        /// </summary>
        private uint m_read_value;
        /// <summary>
        /// The value to be reported as output
        /// </summary>
        private uint m_write_value;

        /// <summary>
        /// The method invoked when all inputs are ready.
        /// The method is only invoked once pr. clock cycle
        /// </summary>
        protected override void OnTick()
        {
            switch (m_state)
            {
                case ControlStates.WaitWrite:
                    Output.OutputValid = true;
                    Output.OutputReady = false;

                    // Wait for output to be consumed
                    if (Input.InputReady)
                    {
                        Output.OutputValid = false;
                        m_state = ControlStates.Init;
                    }
                    break;

                case ControlStates.Compute:
                    m_write_value = m_read_value << 16 | m_read_value >> 16;
                    Output.OutputValid = true;
                    Output.OutputReady = false;
                    m_state = ControlStates.WaitWrite;

                    break;

                case ControlStates.WaitRead:
                    Output.OutputValid = false;
                    Output.OutputReady = true;

                    // Wait for input to arrive
                    if (Input.InputValid)
                    {
                        m_read_value = Input.Value;
                        Output.OutputReady = false;
                        m_state = ControlStates.Compute;
                    }
                    break;

                case ControlStates.Init:
                default:
                    Output.OutputValid = false;
                    Output.OutputReady = true;
                    m_state = ControlStates.WaitRead;
                    break;
            }

            Output.Value = m_write_value;
        }
    }
}
