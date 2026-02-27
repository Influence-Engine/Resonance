namespace Resonance.Oscillators
{
    public abstract class OscillatorBase : IOscillator
    {
        protected float phase;
        protected float phaseIncrement;

        protected readonly AudioFormat format;

        public float Frequency { get; protected set; }

        protected OscillatorBase(AudioFormat format)
        {
            this.format = format;
        }

        public virtual void SetFrequency(float frequencyHz)
        {
            Frequency = frequencyHz;
            phaseIncrement = format.FrequencyToPhaseIncrement(frequencyHz);
        }

        public abstract float NextSample();

        protected void AdvancePhase()
        {
            phase += phaseIncrement;
            if (phase >= 1f)
                phase -= 1f;
        }

        public virtual void Reset() => phase = 0;
    }
}
