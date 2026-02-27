namespace Resonance.Filters
{
    /// <summary>Simple one-pole filter (6dB/octave)</summary>
    public class OnePoleFilter : FilterBase
    {
        // Coefficients
        float a0 = 1;
        float b1;

        // Filter state
        float z1;

        float _frequency;
        public float Frequency
        {
            get => _frequency;
            set
            {
                _frequency = ClampFrequency(value);
                UpdateCoefficients();
            }
        }

        public OnePoleFilter(int sampleRate, FilterType type, float frequency) : base(sampleRate, type)
        {
            this._frequency = ClampFrequency(frequency);
            UpdateCoefficients();
        }

        public override float Process(float input)
        {
            float output = input * a0 + z1;
            z1 = output * b1;

            return type == FilterType.HighPass ? input - output : output;
        }

        public override void Reset() => z1 = 0;

        void UpdateCoefficients()
        {
            float theta = 2f * MathF.PI * _frequency / sampleRate;

            if(type == FilterType.LowPass)
            {
                b1 = MathF.Exp(-theta);
                a0 = 1f - b1;
            }
            else // HighPass
            {
                b1 = (float)MathF.Exp(-theta);
                a0 = 1f - b1;

                // NOTE: For HighPass we invert in Process method
            }
        }
    }
}
