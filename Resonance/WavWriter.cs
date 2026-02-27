
namespace Resonance
{
    public enum AudioEncoding
    {
        Pcm16,
        Pcm24,
        Pcm32F
    }

    public static class WavWriter
    {
        public static byte[] Write(AudioBuffer buffer, AudioEncoding encoding = AudioEncoding.Pcm16)
        {
            byte[] pcm;
            short bitsPerSample;
            short formatCode;
            int bytesPerSample;

            switch(encoding)
            {
                case AudioEncoding.Pcm16:
                    pcm = buffer.ToPcm16();
                    bitsPerSample = 16;
                    formatCode = 1;
                    bytesPerSample = 2;
                    break;

                    case AudioEncoding.Pcm24:
                    pcm = buffer.ToPcm24();
                    bitsPerSample =  24;
                    formatCode = 1;
                    bytesPerSample = 3;
                    break;

                case AudioEncoding.Pcm32F:
                    pcm = buffer.ToPcm16();
                    bitsPerSample = 32;
                    formatCode = 3;
                    bytesPerSample = 4;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(encoding));
            }

            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            int blockAlign = buffer.Format.Channels * bytesPerSample;
            int byteRate = buffer.Format.SampleRate * blockAlign;
            int dataSize = pcm.Length;

            // RIFF
            writer.Write("RIFF"u8.ToArray());
            writer.Write(36 + dataSize);
            writer.Write("WAVE"u8.ToArray());

            // fmt
            writer.Write("fmt "u8.ToArray());
            writer.Write(16); // Chunk Size
            writer.Write(formatCode); // PCM Format
            writer.Write((short)buffer.Format.Channels); // Channels
            writer.Write(buffer.Format.SampleRate); // Sample Rate
            writer.Write(byteRate); // Byte Rate
            writer.Write((short)blockAlign); // Block align
            writer.Write(bitsPerSample); // Bits Per Sample

            // Data
            writer.Write("data"u8.ToArray());
            writer.Write(dataSize);
            writer.Write(pcm);

            return stream.ToArray();
        }
    }
}
