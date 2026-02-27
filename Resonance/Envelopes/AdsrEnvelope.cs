
namespace Resonance.Envelopes
{
    /// <summary>Attack-Decay-Sustain-Release envelope generator</summary>
    public class AdsrEnvelope
    {
        enum EnvelopeStage
        {
            Off,
            Attack,
            Decay,
            Sustain,
            Release
        }

        EnvelopeStage stage = EnvelopeStage.Off;
        float currentLevel;
        int samplesInCurrentStage;
        int totalSamplesForCurrentStage;
        AudioFormat format;

        public float AttackTime = 0.01f;
        public float DecayTime = 0.1f;
        public float SustainLevel = 0.7f;
        public float ReleaseTime = 0.2f;

        public float AttackCurve = 1f;
        public float DecayCurve = 1f;
        public float ReleaseCurve = 1f;

        public AdsrEnvelope(AudioFormat format) => this.format = format;

        // same same but different <.<
        public void SetSampleRate(AudioFormat format) => this.format = format;

        public void NoteOn()
        {
            stage = EnvelopeStage.Attack;
            currentLevel = 0;
            samplesInCurrentStage = 0;
            totalSamplesForCurrentStage = format.SecondsToSamples(AttackTime); // AttackTime
        }

        public void NoteOff()
        {
            if(stage != EnvelopeStage.Off && stage != EnvelopeStage.Release)
            {
                stage = EnvelopeStage.Release;
                samplesInCurrentStage = 0;
                totalSamplesForCurrentStage = format.SecondsToSamples(ReleaseTime); // ReleaseTime
            }
        }

        public float Process()
        {
            if (stage == EnvelopeStage.Off)
                return 0;

            float progress = totalSamplesForCurrentStage > 0
                ? samplesInCurrentStage / (float)totalSamplesForCurrentStage
                : 1f;

            currentLevel = stage switch
            {
                EnvelopeStage.Attack => ApplyCurve(progress, AttackCurve),
                EnvelopeStage.Decay => 1f - ApplyCurve(progress, DecayCurve) * (1f - SustainLevel),
                EnvelopeStage.Sustain => SustainLevel,
                EnvelopeStage.Release => SustainLevel * (1f - ApplyCurve(progress, ReleaseCurve)),

                _ => 0,
            };

            samplesInCurrentStage++;
            if (samplesInCurrentStage >= totalSamplesForCurrentStage)
                AdvanceStage();

            return currentLevel;
        }

        void AdvanceStage()
        {
           switch(stage)
            {
                case EnvelopeStage.Attack:
                    stage = EnvelopeStage.Decay;
                    samplesInCurrentStage = 0;
                    totalSamplesForCurrentStage = format.SecondsToSamples(DecayTime);
                    break;
                case EnvelopeStage.Decay:
                    stage = EnvelopeStage.Sustain;
                    samplesInCurrentStage = 0;
                    totalSamplesForCurrentStage = int.MaxValue;
                    break;
                case EnvelopeStage.Release:
                    stage = EnvelopeStage.Off;
                    currentLevel = 0;
                    break;
            }
        }

        float GetCurrentStageTime()
        {
            return stage switch
            {
                EnvelopeStage.Attack => AttackTime,
                EnvelopeStage.Decay => DecayTime,
                EnvelopeStage.Release => ReleaseTime,
                _ => 0
            };
        }

        float ApplyCurve(float t, float curve) => MathF.Pow(t, curve);

        public void Reset()
        {
            stage = EnvelopeStage.Off; 
            currentLevel = 0;
            samplesInCurrentStage = 0;
            totalSamplesForCurrentStage = 0;
        }
    }
}
