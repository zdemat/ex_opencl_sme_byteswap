using SME;

namespace ByteSwap
{
	/// <summary>
	/// Output for a lite avalon interface
	/// </summary>
	public interface ILiteAvalonOutput : IBus
	{

		/// <summary>
		/// A value that signals that the receiver can accept data
		/// </summary>
		[InitialValue(true)]
		bool OutputReady { get; set; } 

        /// <summary>
        /// A value that signals that the value is set
        /// </summary>
		[InitialValue]
		bool OutputValid { get; set; }

		/// <summary>
		/// The value being reported by this module
		/// </summary>
		uint Value { get; set; }
	}

	public interface ILiteAvalonInput : IBus
	{
		/// <summary>
		/// A signal that is set when value can be consumed
		/// </summary>
		[InitialValue]
		bool InputReady { get; set; }		

		/// <summary>
		/// A signal that is set when the value can be read
		/// </summary>
		/// <value></value>
		[InitialValue]
		bool InputValid { get; set; }

		/// <summary>
		/// The value to read
		/// </summary>
		uint Value { get; set; }
	}
}