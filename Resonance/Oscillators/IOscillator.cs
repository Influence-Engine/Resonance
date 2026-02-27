namespace Resonance.Oscillators
{
    // Petition to call it IResonator >.<
    /// <summary>Interface for Oscillators which generate a periodic waveform</summary>
    public interface IOscillator
    {
        /// <summary>Generate the next sample</summary>
        float NextSample();

        /// <summary>Set frequency in Hz</summary>
        void SetFrequency(float frequencyHz);

        /// <summary>Reset the phase</summary>
        void Reset();
    }
}
