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
            Stall,
            /// <summary>The protocol is forwarding data</summary>
            Forward
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
                // In this state we keep sending the input to the output
                case ControlStates.Forward:

                    // Check if the upstream is delivering more data
                    if (Input.InputValid)
                    {
                        // Forward the data
                        Output.Value = Input.Value << 16 | Input.Value >> 16;
                        Output.OutputValid = true;

                        // If downstream accepts it, continue forwarding
                        Output.OutputReady = Input.InputReady;

                        // If downstream is stalling, stop the forwarding
                        if (!Input.InputReady)
                            m_state = ControlStates.Start;
                    }
                    else
                    {
                        // No more data from upstream
                        Output.OutputValid = false;
                        Output.OutputReady = true;
                        m_state = ControlStates.Start;
                    }
                    break;

                // In this state, the downstream module has stalled
                case ControlStates.Stall:
                    // Wait for the downstream module to activate
                    if (Input.InputReady)
                        m_state = ControlStates.Forward;
                    break;

                //case ControlStates.Start:
                default:
                    // Wait for input to arrive
                    if (Input.InputValid)
                    {
                        // Forward the output (will be latched
                        Output.Value = Input.Value << 16 | Input.Value >> 16;
                        Output.OutputValid = true;

                        if (Input.InputReady)
                        {
                            // If downstream can read, start forwarding mode
                            Output.OutputReady = true;
                            m_state = ControlStates.Forward;
                        }
                        else
                        {
                            // If downstream is blocking, enter stall mode
                            Output.OutputReady = false;
                            m_state = ControlStates.Stall;
                        }
                    }

                    break;
            }
        }
    }
}
