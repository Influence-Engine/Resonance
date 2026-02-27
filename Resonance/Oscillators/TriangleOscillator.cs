
namespace Resonance.Oscillators
{
    /// <summary>Triangle wave oscillator</summary>
    public class TriangleOscillator : OscillatorBase
    {
        public TriangleOscillator(AudioFormat format) : base(format) { }

        public override float NextSample()
        {
            // Rising: -1 to 1
            // Falling: 1 to -1
            float sample = phase < 0.5f ? 4f * phase - 1f : 3f - 4f * phase;
            AdvancePhase();

            return sample;
        }
    }
}
