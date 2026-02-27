using System.Text;

namespace Resonance
{
    // TODO make WavWriter
    // TODO add ToPcm24 and ToPcm32F (only use byte[])
    public class AudioBuffer
    {
        readonly float[] samples;
        public AudioFormat Format { get; }

        public int Length => samples.Length;
        public Span<float> Samples => samples.AsSpan();

        public float DurationSeconds => samples.Length / (float)(Format.SampleRate * Format.Channels);

        public AudioBuffer(AudioFormat format, int sampleCount)
        {
            if (sampleCount % format.Channels != 0)
                throw new ArgumentException("Sample count must be divisible by channel count");

            this.Format = format;
            this.samples = new float[sampleCount];
        }

        public byte[] ToPcm16()
        {
            byte[] pcm = new byte[samples.Length * 2];
            int offset = 0;

            for (int i = 0; i < samples.Length; i++)
            {
                float clamped = Math.Clamp(samples[i], -1f, 1f);
                short value = (short)MathF.Round(clamped * 32767f); // short.MaxValue => 32767

                pcm[offset++] = (byte)(value & 0xFF);
                pcm[offset++] = (byte)((value >> 8) & 0xFF);
            }

            return pcm;
        }

        public byte[] ToPcm24()
        {
            byte[] pcm = new byte[samples.Length * 3];
            int offset = 0;

            for (int i = 0; i < samples.Length; i++)
            {
                float clamped = Math.Clamp(samples[i], -1f, 1f);
                int value = (int)MathF.Round(clamped * 8388607f); // signed 24-bit.MaxValue >= 8388607

                pcm[offset++] = (byte)(value & 0xFF);
                pcm[offset++] = (byte)((value >> 8) & 0xFF);
                pcm[offset++] = (byte)((value >> 16) & 0xFF);
            }

            return pcm;
        }

        public byte[] ToPcm32F()
        {
            byte[] pcm = new byte[samples.Length * 4];
            Buffer.BlockCopy(samples, 0, pcm, 0, pcm.Length);
            return pcm;
        }

        public void ApplyGain(float gain)
        {
            for (int i = 0; i < samples.Length; i++)
                samples[i] *= gain;
        }

        public void Normalize()
        {
            float max = 0f;
            foreach(float s in samples)
                max = MathF.Max(max, MathF.Abs(s));

            if (max <= 0f)
                return;

            float scale = 1f / max;

            for(int i = 0; i < samples.Length; i++)
                samples[i] *= scale;
        }

        public void Clear() => samples.AsSpan().Clear();
    }
}
