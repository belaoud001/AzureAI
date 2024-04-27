using Microsoft.CognitiveServices.Speech.Audio;

namespace AzureAiAPI.Helpers;

public class BinaryAudioStreamReader : PullAudioInputStreamCallback
{

    private BinaryReader _binaryReader;
    private bool disposed = false;

    public BinaryAudioStreamReader(BinaryReader binaryReader)
    {
        _binaryReader = binaryReader;
    }
    
    public BinaryAudioStreamReader(Stream stream) : this(new BinaryReader(stream)) { }

    public override int Read(byte[] dataBuffer, uint size)
    {
        return _binaryReader.Read(dataBuffer, 0, (int) size);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposed) return;
        
        if (disposing)
            _binaryReader.Dispose();

        disposed = true;
        base.Dispose(disposing);
    }

}
