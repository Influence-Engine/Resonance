
namespace Resonance.Oscillators
{
    /// <summary>Square wave oscillator</summary>
    public class SquareOscillator : OscillatorBase
    {
        float _pulseWidth = 0.5f;
        public float PulseWidth
        {
            get => _pulseWidth;
            set => _pulseWidth = Math.Clamp(value, 0.01f, 0.99f);
        }

        public SquareOscillator(AudioFormat format) : base(format) { }

        public override float NextSample()
        {
            float sample = phase < _pulseWidth ? 1f : -1f;
            AdvancePhase();

            return sample;
        }
    }
}
