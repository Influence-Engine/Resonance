# Resonance
A simple procedural audio generator written in C#.  
Give it some parameters, get back audio as a `.wav` file or raw `byte[]` data which you can use wherever needed.

## Features
- Procedural Sound Generation: Describe a sound with oscillator, envelope, and filter settings.
- Raw Byte Output: Get `byte[]` data for real-time use, streaming, etc.
- Wave File Output: Convert `byte[]` directly to `.wav`.
- 3 Encoding Formats: PCM 16-bit, PCM 24-bit, IEEE 32-bit float.
- And many more!

## Getting Started
Resonance is distributed as a `.dll` library. Add a reference to `Resonance.dll` in your project and you are ready to create audio.
> Targets netstandard2.1 for broad compatibility (.NET, Mono, Unity, Godot, etc.)

## Usage
### Generate a WAV file
```cs
using Resonance;
using Resonance.Oscillators;

SoundEffect effect = new SoundEffect(AudioFormat.CDMono)
{
  WaveType = OscillatorType.Square,
  StarFrequency = 1000f,
  EndFrequency = 1400f,
  AttackTime = 0.001f,
  DecayTime = 0.1f,
  SustainLevel = 0f,
  Volume = 0.5f
};

AudioBuffer buffer = effect.Generate(duration: 0.5f);
byte[] wav = WavWriter.Write(buffer, AudioEncoding.Pcm16);
File.WriteAllBytes("Coin.wav", wav);
```

### Use raw bytes directly
```cs
byte[] wav = WavWriter.Write(buffer, AudioEncoding.Pcm32F);
// Pass wav to your audio system, stream it, write it to disk... up to you
```

### Add Filters
```cs
using Resonance.Filters;

effect.AddFilter(new OnePoleFilter(effect.SampleRate, FilterType.LowPass, frequency: 200f));
AudioBuffer buffer = effect.Generate(1f);
```

## Roadmap
- [ ] User Interface
- [ ] More Filter Types
- [ ] Unity Editor Window (Seperate Project)
