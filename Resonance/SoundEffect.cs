using Resonance.Oscillators;
using Resonance.Envelopes;
using Resonance.Filters;
using System.Collections.ObjectModel;

namespace Resonance
{
    /// <summary>Represents a complete sound effect with all parameters</summary>
    public class SoundEffect
    {
        readonly AudioFormat format;
        readonly List<IFilter> filters = new();

        public IReadOnlyList<IFilter> Filters => new ReadOnlyCollection<IFilter>(filters);

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

        // Modulation
        public float VibratoFrequency = 5f;
        public float VibratoDepth = 0f;

        // Pulse width (for square waves)
        public float PulseWidth = 0.5f;
        public float PulseWidthSweep = 0f;

        // Effects
        public float Overdrive = 0f;

        // Volume
        public float Volume = 0.5f;

        public int SampleRate => format.SampleRate;

        public SoundEffect(AudioFormat format) => this.format = format;

        /// <summary>Generate the sound effect as an audio buffer</summary>
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

                buffer.Samples[i] = sample;
            }

            foreach (IFilter filter in filters)
            {
                filter.Reset();
                filter.ProcessBlock(buffer.Samples[..sampleCount]);
            }

            if (Overdrive > 0)
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    float sample = buffer.Samples[i];
                    sample = MathF.Tanh(sample * Overdrive) / Overdrive;
                    buffer.Samples[i] = sample;
                }
            }

            for (int i = 0; i < sampleCount; i++)
            {
                buffer.Samples[i] *= Volume;
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

        #region Filters

        public void AddFilter(IFilter filter)
        {
            if (filter.SampleRate != format.SampleRate)
                throw new ArgumentException($"Filter sample rate ({filter.SampleRate}) must match effect sample rate ({format.SampleRate})");

            filters.Add(filter);
        }

        public void InsertFilter(int index, IFilter filter)
        {
            if (filter.SampleRate != format.SampleRate)
                throw new ArgumentException($"Filter sample rate ({filter.SampleRate}) must match effect sample rate ({format.SampleRate})");

            filters.Insert(index, filter);
        }

        public bool RemoveFilter(IFilter filter) => filters.Remove(filter);
        public void RemoveFilterAt(int index) => filters.RemoveAt(index);

        public void ClearFilters() => filters.Clear();

        #endregion
    }
}
