
namespace Resonance.Oscillators
{
    /// <summary>Sawtooth wave oscillator</summary>
    public class SawtoothOscillator : OscillatorBase
    {
        public SawtoothOscillator(AudioFormat format) : base(format) { }

        public override float NextSample()
        {
            float sample = 2f * phase - 1f;
            AdvancePhase();

            return sample;
        }
    }
}
