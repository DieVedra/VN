using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "AudioData/AudioData", order = 51)]
public class AudioData : ScriptableObject
{
    [SerializeField] private List<AudioClip> _audioClips;
    public IReadOnlyList<AudioClip> Clips => _audioClips;

}