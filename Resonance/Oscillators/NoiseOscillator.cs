
namespace Resonance.Oscillators
{
    /// <summary>Its just white noise</summary>
    public class NoiseOscillator : OscillatorBase
    {
        readonly Random random;

        public NoiseOscillator(AudioFormat format, int? seed = null) : base(format)
        {
            this.random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public override float NextSample() => (float)(random.NextDouble() * 2.0 - 1.0);

        // These don't do anything (I think)
        public override void SetFrequency(float frequencyHz) { }
        public override void Reset() { }
    }
}
