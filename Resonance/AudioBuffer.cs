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

        public short[] ToPcm16()
        {
            short[] pcm = new short[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                // Clamp to avoid clipping
                float sample = Math.Clamp(samples[i], -1f, 1f);
                pcm[i] = (short)MathF.Round(sample * short.MaxValue);
            }

            return pcm;
        }

        public byte[] ToPcm16Byte()
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

        public byte[] ToWavBytes()
        {
            var pcm = ToPcm16();
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            int byteRate = Format.SampleRate * Format.BytesPerSample * Format.Channels;
            int dataSize = pcm.Length * Format.BytesPerSample;

            // RIFF header
            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + dataSize);
            writer.Write(Encoding.ASCII.GetBytes("WAVE"));

            // fmt chunk
            writer.Write(Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16); // chunk size
            writer.Write((short)1); // PCM Format
            writer.Write((short)Format.Channels);
            writer.Write(Format.SampleRate);
            writer.Write(byteRate);
            writer.Write((short)(Format.BytesPerSample * Format.Channels)); // Block align
            writer.Write((short)16);

            // Data chunk
            writer.Write(Encoding.ASCII.GetBytes("data"));
            writer.Write(dataSize);

            foreach(short sample in pcm)
            {
                writer.Write(sample);
            }

            writer.Flush();
            return stream.ToArray();
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
