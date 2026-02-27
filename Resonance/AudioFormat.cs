namespace Resonance
{
    public readonly struct AudioFormat
    {
        public int SampleRate { get; }
        public int Channels { get; }
        public int BytesPerSample => 2; // 16-bit PCM

        public AudioFormat(int sampleRate = 44100, int channels = 1)
        {
            if (sampleRate <= 0)
                throw new ArgumentException("Sample rate must be positive");

            if (channels <= 0)
                throw new ArgumentException("Channels must be positive");

            this.SampleRate = sampleRate;
            this.Channels = channels;
        }

        public int SecondsToSamples(float seconds) => (int)(seconds * SampleRate);

        public float SamplesToSeconds(int samples) => samples / (float)SampleRate;

        public float FrequencyToPhaseIncrement(float frequencyHz) => frequencyHz / SampleRate;


        public static readonly AudioFormat CDMono = new AudioFormat(44100, 1);
        public static readonly AudioFormat CDStereo = new AudioFormat(44100, 2);
        public static readonly AudioFormat StudioMono = new AudioFormat(48000, 1);
        public static readonly AudioFormat StudioStereo = new AudioFormat(48000, 2);
    }
}
