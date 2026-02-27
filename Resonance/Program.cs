using Resonance;
using Resonance.Oscillators;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Resonance");

        AudioFormat format = AudioFormat.CDMono;

        var effect1 = new SoundEffect(format)
        {
            WaveType = OscillatorType.Square,
            StartFrequency = 800f,
            EndFrequency = 200f,
            AttackTime = 0.001f,
            DecayTime = 0.1f,
            SustainLevel = 0,
            PulseWidth = 0.5f,
            Volume = 0.5f
        };

        SaveSound(effect1.Generate(1f), "effect1.wav");
        Console.WriteLine("Saved effect1.wav");

        var effect2 = new SoundEffect(format)
        {
            WaveType = OscillatorType.Noise,
            StartFrequency = 200f,
            EndFrequency = 50f,
            AttackTime = 0.001f,
            DecayTime = 1f,
            SustainLevel = 0,
            Volume = 0.5f
        };

        SaveSound(effect2.Generate(1f), "effect2.wav");
        Console.WriteLine("Saved effect2.wav");

        Console.ReadKey();
    }

    static void SaveSound(AudioBuffer buffer, string fileName)
    {
        byte[] data = WavWriter.Write(buffer, AudioEncoding.Pcm32F);
        File.WriteAllBytes(fileName, data);
    }
}