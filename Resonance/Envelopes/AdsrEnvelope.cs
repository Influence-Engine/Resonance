using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

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
        int samplesRemaining;
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
            samplesRemaining = format.SecondsToSamples(0.01f); // AttackTime
        }

        public void NoteOff()
        {
            if(stage != EnvelopeStage.Off)
            {
                stage = EnvelopeStage.Release;
                samplesRemaining = format.SecondsToSamples(0.2f); // ReleaseTime
            }
        }

        public float Process()
        {
            if (stage == EnvelopeStage.Off)
                return 0;

            if (samplesRemaining <= 0)
                AdvanceStage();

            samplesRemaining--;

            float progess = 1f - (samplesRemaining / (float)format.SecondsToSamples(GetCurrentStageTime()));

            currentLevel = stage switch
            {
                EnvelopeStage.Attack => ApplyCurve(progess, AttackCurve),
                EnvelopeStage.Decay => 1f - ApplyCurve(progess, DecayCurve) * (1f - SustainLevel),
                EnvelopeStage.Sustain => SustainLevel,
                EnvelopeStage.Release => SustainLevel * (1f - ApplyCurve(progess, ReleaseCurve)),

                _ => 0,
            };

            return currentLevel;
        }

        void AdvanceStage()
        {
           switch(stage)
            {
                case EnvelopeStage.Attack:
                    stage = EnvelopeStage.Decay;
                    samplesRemaining = format.SecondsToSamples(DecayTime);
                    currentLevel = 1f;
                    break;
                case EnvelopeStage.Decay:
                    stage = EnvelopeStage.Sustain;
                    samplesRemaining = int.MaxValue;
                    break;
                case EnvelopeStage.Release:
                    stage = EnvelopeStage.Off;
                    samplesRemaining = 0;
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

        float ApplyCurve(float t, float curve) => MathF.Pow(t, curve) * curve;

        public void Reset()
        {
            stage = EnvelopeStage.Off; 
            currentLevel = 0;
            samplesRemaining = 0;
        }
    }
}
