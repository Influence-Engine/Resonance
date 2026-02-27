
namespace Resonance.Filters
{
    /// <summary>Chain multiple filters together</summary>
    public class FilterChain : IFilter
    {
        readonly List<IFilter> filters = new();
        readonly int sampleRate;

        public int SampleRate => sampleRate;
        public FilterType Type => FilterType.Chain;
        public int Count => filters.Count;

        public FilterChain(int sampleRate) => this.sampleRate = sampleRate;

        public void AddFilter(IFilter filter)
        {
            if (filter.SampleRate != sampleRate)
                throw new ArgumentException("FIlter sample rate must match chain sample rate");

            filters.Add(filter);
        }

        public void RemoveFilter(IFilter filter) => filters.Remove(filter);

        public void RemoveAt(int index) => filters.RemoveAt(index);

        public IFilter? GetFilter(int index) => index >= 0 && index < filters.Count ? filters[index] : null;

        public void Clear() => filters.Clear();

        #region IFilter Interface

        public float Process(float input)
        {
            float output = input;
            foreach(var filter in filters)
            {
                output = filter.Process(output);
            }
            return output;
        }

        public void ProcessBlock(Span<float> samples)
        {
            foreach(var filter in filters)
            {
                filter.ProcessBlock(samples);
            }
        }

        public void Reset()
        {
            foreach (var filter in filters)
            {
                filter.Reset();
            }
        }

        #endregion
    }
}
