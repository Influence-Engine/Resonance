
namespace Resonance
{
    public class AudioBuffer
    {
        readonly float[] samples;
        public AudioFormat format;

        public int Length => samples.Length;
        public Span<float> Samples => samples.AsSpan();

        public AudioBuffer(AudioFormat format, int sampleCount)
        {
            this.format = format;
            this.samples = new float[sampleCount];
        }

        public short[] ToPcm16()
        {
            short[] pcm = new short[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                // Clamp to avoid clipping
                float sample = Math.Clamp(samples[i], -1f, 1f);
                pcm[i] = (short)(sample * short.MaxValue);
            }

            return pcm;
        }
    }
}
