
public class GlobalSound : Sound
{
    private AudioClipProvider _audioClipProvider;
    public void Construct(bool soundOn)
    {
        Init(soundOn);
    }

    public void SetAudioClipProvider(AudioClipProvider audioClipProvider)
    {
        _audioClipProvider = audioClipProvider;
        MusicAudioData = _audioClipProvider.MusicAudioData;
        AmbientAudioData = _audioClipProvider.AmbientAudioData;
    }
    public override void Dispose()
    {
        _audioClipProvider.Dispose();
        base.Dispose();
    }
    public void SetGlobalSoundData(GlobalAudioData globalAudioData)
    {
        GlobalAudioData = globalAudioData;
    }
}
