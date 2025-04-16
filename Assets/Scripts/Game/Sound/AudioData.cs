using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicAudioData", menuName = "MusicAudioData/MusicAudioData", order = 51)]
public class AudioData : ScriptableObject
{
    [SerializeField] private List<AudioClip> _audioClips;
    public IReadOnlyList<AudioClip> Clips => _audioClips;

}