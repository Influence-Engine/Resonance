using Resonance;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Resonance");

        AudioFormat format = AudioFormat.CDMono;

        // new SoundEffect(format) would be cool
        // then just SaveSound(sound.Generate(length), name)
    }

    static void SaveSound(AudioBuffer buffer, string fileName)
    {
        byte[] data = buffer.ToWavBytes();
        File.WriteAllBytes(fileName, data);
    }
}