namespace Resonance.Filters
{
    /// <summary>Base interface for all audio filters</summary>
    public interface IFilter
    {
        int SampleRate { get; }
        FilterType Type { get; }

        /// <summary>Process a single sample through the filter </summary>
        float Process(float input);

        /// <summary>Process a block of samples in-place</summary>
        void ProcessBlock(Span<float> samples);

        /// <summary>Reset the filters internal state</summary>
        void Reset();
    }
}
