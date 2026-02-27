namespace Resonance
{
    // Petition to call it IResonator >.<
    public interface IOscillator
    {
        float NextSample();

        void SetFrequency(float frequencyHz);

        void Reset();
    }
}
