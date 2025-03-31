
using UnityEngine;

public class GlobalSound : Sound
{
    [SerializeField] private AudioClip _wardrobeAudioClip;
    
    public void SetAudioData(AudioData audioData)
    {
        AddAudioClips(audioData.Clips);
    }
}
