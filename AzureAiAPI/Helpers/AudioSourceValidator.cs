using AzureAiAPI.Exceptions;
using NAudio.Wave;

namespace AzureAiAPI.Helpers;

public class AudioSourceValidator
{
    
    public static async Task<Stream> ExtractMemoryStream(IFormFile audioSource)
    {
        if (Path.GetExtension(audioSource.FileName).ToLower() != ".wav")
        {
            throw new FileFormatNotSupported();
        }

        var memoryStream = new MemoryStream();
        
        await audioSource.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        using (var waveFileReader = new WaveFileReader(memoryStream))
        {
            int bitsPerSecond = waveFileReader.WaveFormat.AverageBytesPerSecond * 8;
                
            if (bitsPerSecond != 256000 && bitsPerSecond != 128000)
            {
                var outStream = new MemoryStream();
                var waveFileWriter = new WaveFileWriter(outStream, new WaveFormat(16000, 16, 1));
                    
                await waveFileReader.CopyToAsync(waveFileWriter); 
                outStream.Seek(0, SeekOrigin.Begin); 
                    
                return outStream;
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
    
}
