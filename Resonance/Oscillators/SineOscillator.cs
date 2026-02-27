
namespace Resonance.Oscillators
{
    /// <summary>Sine wave oscillator</summary>
    public class SineOscillator : OscillatorBase
    {
        public SineOscillator(AudioFormat format) : base(format) { }

        public override float NextSample()
        {
            float sample = (float)Math.Sin(phase * 2.0 * Math.PI);
            AdvancePhase();

            return sample;
        }
    }
}
