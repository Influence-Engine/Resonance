namespace Resonance.Filters
{
    public interface IFilter
    {
        int SampleRate { get; }
        FilterType Type { get; }

        float Process(float input);

        void ProcessBlock(Span<float> samples);

        void Reset();
    }
}
