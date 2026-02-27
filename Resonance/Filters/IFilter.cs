using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Resonance.Filters
{
    public enum FilterType
    {
        LowPass,
        HighPass,
        BandPass,
        // Let me google more later lol
    }

    public interface IFilter
    {
        FilterType Type { get; }
        int SampleRate { get; }

        float Process(float input);

        void ProcessBlock(Span<float> samples);

        void Reset();
    }
}
