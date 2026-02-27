using System.Text;

namespace Resonance
{
    public class AudioBuffer
    {
        readonly float[] samples;
        public AudioFormat format;

        public int Length => samples.Length;
        public Span<float> Samples => samples.AsSpan();

        public float DurationSeconds => samples.Length / (float)(format.SampleRate * format.Channels);

        public AudioBuffer(AudioFormat format, int sampleCount)
        {
            if (sampleCount % format.Channels != 0)
                throw new ArgumentException("Sample count must be divisible by channel count");

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

        public byte[] ToWavBytes()
        {
            var pcm = ToPcm16();
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            int byteRate = format.SampleRate * format.BytesPerSample * format.Channels;
            int dataSize = pcm.Length * format.BytesPerSample;

            // RIFF header
            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + dataSize);
            writer.Write(Encoding.ASCII.GetBytes("WAVE"));

            // fmt chunk
            writer.Write(Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16); // chunk size
            writer.Write((short)1); // PCM Format
            writer.Write((short)format.Channels);
            writer.Write(format.SampleRate);
            writer.Write(byteRate);
            writer.Write((short)(format.BytesPerSample * format.Channels)); // Block align
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

        public void Clear() => samples.AsSpan().Clear();
    }
}
