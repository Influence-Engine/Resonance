using System;
using System.Collections.Generic;
using System.Text;

namespace Resonance.Filters
{
    /// <summary>Base class for all filters with common functionality</summary>
    public abstract class FilterBase : IFilter
    {
        protected readonly int sampleRate;
        protected FilterType type;
        public int SampleRate => sampleRate;
        public FilterType Type => type;

        public FilterBase(int sampleRate, FilterType type)
        {
            this.sampleRate = sampleRate;
            this.type = type;
        }

        public abstract float Process(float input);

        public virtual void ProcessBlock(Span<float> samples)
        {
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = Process(samples[i]);
            }
        }

        public abstract void Reset();

        protected float ClampFrequency(float frequency) => Math.Clamp(frequency, 20f, sampleRate / 2.5f);
    }
}
