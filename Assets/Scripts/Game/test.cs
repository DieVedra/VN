using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    [SerializeField] private AudioData _audioData;
    [SerializeField] private AudioSource _audioSource;

    private AsyncOperationHandle<AudioData> _asyncOperationHandle;



    public void tst()
    {
        Application.Quit();
    }
    [Button()]
    private void Set()
    {
        Load().Forget();
    }

    private async UniTask Load()
    {
        
        _asyncOperationHandle = Addressables.LoadAssetAsync<AudioData>("MusicAudioData");
        await _asyncOperationHandle.Task;
        if (_asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            _audioData = _asyncOperationHandle.Result;
            
        }
    }

    [Button()]
    private void Remote()
    {
        Addressables.ReleaseInstance(_asyncOperationHandle);
        _audioData = null;
    }

    [Button()]
    private void Play()
    {
        _audioSource.clip = _audioData.Clips[0];
        _audioSource.Play();
    }
    [Button()]
    private void Stop()
    {
        _audioSource.Stop();
        _audioSource.clip = null;
    }
}