using Resonance.Oscillators;
using Resonance.Envelopes;

namespace Resonance
{
    public class SoundEffect
    {
        readonly AudioFormat format;

        // Oscillator settings
        public OscillatorType WaveType = OscillatorType.Square;
        public float StartFrequency = 440f;
        public float EndFrequency = 440f;
        public float MinFrequency = 20f;

        // Envelope settings
        public float AttackTime = 0.01f;
        public float DecayTime = 0.1f;
        public float SustainLevel = 0.7f;
        public float ReleaseTime = 0.2f;
        public float HoldTime = 0f;

        public float VibratoFrequency = 5f;
        public float VibratoDepth = 0f;

        public float PulseWidth = 0.5f;
        public float PulseWidthSweep = 0f;

        public float Overdrive = 0f;

        public float Volume = 0.5f;

        public SoundEffect(AudioFormat format) => this.format = format;

        public AudioBuffer Generate(float duration)
        {
            int sampleCount = format.SecondsToSamples(duration);
            AudioBuffer buffer = new AudioBuffer(format, sampleCount);

            var envelope = new AdsrEnvelope(format)
            {
                AttackTime = AttackTime,
                DecayTime = DecayTime,
                SustainLevel = SustainLevel,
                ReleaseTime = ReleaseTime,
            };

            var oscillator = CreateOscillator();
            oscillator.SetFrequency(StartFrequency);

            float currentFrequency = StartFrequency;
            float frequencySliderPerSample = (EndFrequency - StartFrequency) / sampleCount;

            // Figure out how to do this only when Square 
            float pulseWidth = PulseWidth;
            float pulseWidthPerSample = PulseWidthSweep / format.SampleRate;

            envelope.NoteOn();

            for (int i = 0; i < sampleCount; i++)
            {
                currentFrequency += frequencySliderPerSample;
                if (currentFrequency < MinFrequency)
                    currentFrequency = MinFrequency;

                oscillator.SetFrequency(currentFrequency);

                if (VibratoDepth > 0)
                {
                    float vibratoOffset = (float)MathF.Sin(i * 2f * MathF.PI * VibratoFrequency / format.SampleRate) * VibratoDepth;
                    oscillator.SetFrequency(currentFrequency + vibratoOffset);
                }

                if (oscillator is SquareOscillator square)
                {
                    // be there or be square o.o
                    pulseWidth += pulseWidthPerSample;
                    square.PulseWidth = pulseWidth;
                }

                float sample = oscillator.NextSample();

                float envelopeLevel = envelope.Process();
                sample *= envelopeLevel;

                if (Overdrive > 0)
                {
                    sample = MathF.Tanh(sample * Overdrive) / Overdrive;
                }

                sample *= Volume;
                buffer.Samples[i] = sample;
            }

            return buffer;
        }

        IOscillator CreateOscillator()
        {
            return WaveType switch
            {
                OscillatorType.Square => new SquareOscillator(format),
                OscillatorType.Sawtooth => new SawtoothOscillator(format),
                OscillatorType.Sine => new SineOscillator(format),
                OscillatorType.Triangle => new TriangleOscillator(format),
                OscillatorType.Noise => new NoiseOscillator(format), // no seed :(
                _ => new SquareOscillator(format)
            };
        }
    }
}
