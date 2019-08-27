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

        /// <summary>
        /// The different states the protocol can be in
        /// </summary>
        private enum ControlStates
        {
            /// <summary>The protocol is waiting for input</summary>
            Start,
            /// <summary>The protocol is blocked by the downstream not accepting input</summary>
            Stall
        }

        /// <summary>
        /// The current state of the process
        /// </summary>
        private ControlStates m_state = ControlStates.Start;


        /// <summary>
        /// The method invoked when all inputs are ready.
        /// The method is only invoked once pr. clock cycle
        /// </summary>
        protected override void OnTick()
        {
            switch (m_state)
            {
                // In this state, the downstream module has stalled
                case ControlStates.Stall:
                    // Wait for the downstream module to activate
                    if (Input.InputReady)
                    {
                        Output.OutputValid = false;
                        Output.OutputReady = true;
                        m_state = ControlStates.Start;
                    }

                    break;

                //case ControlStates.Start:
                default:
                    Output.OutputValid = Input.InputValid;

                    // Wait for input to arrive
                    if (Input.InputValid)
                    {
                        // Forward the output (will be latched
                        Output.Value = Input.Value << 16 | Input.Value >> 16;
                        Output.OutputReady = Input.InputReady;

                        // If downstream can read, start forwarding mode
                        if (!Input.InputReady)
                            m_state = ControlStates.Stall;
                    }

                    break;
            }
        }
    }
}
