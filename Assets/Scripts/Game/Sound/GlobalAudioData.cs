using UnityEngine;

[CreateAssetMenu(fileName = "GlobalAudioData", menuName = "MusicAudioData/GlobalAudioData", order = 51)]

public class GlobalAudioData : ScriptableObject
{
    [SerializeField] private AudioClip _wardrobeAudioClip;
    
    public AudioClip GetWardrobeAudioClip => _wardrobeAudioClip;
}