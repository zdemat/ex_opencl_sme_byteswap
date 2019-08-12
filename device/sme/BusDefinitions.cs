using SME;

namespace ByteSwap
{
	/// <summary>
	/// A bus for swapping lower and higher part of word
	/// </summary>
	public interface ValueBus : IBus
	{
        // The [InitialValue] attribute makes sure we can read the value without writing.
		// It is also possible to set [InitializedBus] to force all fields to be initialized
		
		/// <summary>
		/// A value indicating the current stored value is valid
		/// </summary>
		[InitialValue]
		bool IsValid { get; set; }

		/// <summary>
		/// A value indicating a new value can be stored
		/// </summary>
		[InitialValue]
		bool IsReady { get; set; }

		/// <summary>
		/// The word value
		/// </summary>
		uint Value { get; set; }
	}
}