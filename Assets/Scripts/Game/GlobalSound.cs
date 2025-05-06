
public class GlobalSound : Sound
{
    private AudioClipProvider _audioClipProvider;
    public void Construct(AudioClipProvider audioClipProvider, bool soundOn)
    {
        Init(soundOn);
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
