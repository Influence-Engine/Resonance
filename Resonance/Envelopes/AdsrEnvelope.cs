using System;
using System.Collections.Generic;
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

        public void Reset()
        {
            stage = EnvelopeStage.Off; 
            currentLevel = 0;
            samplesRemaining = 0;
        }
    }
}
