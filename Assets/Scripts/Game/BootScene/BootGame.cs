
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BootGame : MonoBehaviour
{
    [SerializeField] private LoadScreenUIView loadScreenUIView;

    private LoadScreenUIHandler _loadScreenUIHandler;
    private SceneLoader _sceneLoader;
    private LoadPercentProviderEvent _loadPercentProviderEvent;
    private Camera _camera;
    private void Awake()
    {
        // _camera = Camera.main;
        // Debug.Log($"_camera {_camera.gameObject.name}");
        // _loadPercentProviderEvent = new LoadPercentProviderEvent();
        // _loadScreenUIHandler = new LoadScreenUIHandler(loadScreenUIView, _loadPercentProviderEvent);
        // _sceneLoader = new SceneLoader(_loadPercentProviderEvent);
    }

    // private void Start()
    // {
    //     Load().Forget();
    // }

    // private async UniTaskVoid Load()
    // {
    //     _loadScreenUIHandler.Show();
    //     _sceneLoader.LoadMenuScene().Forget();
    //     await _loadScreenUIHandler.Hide();
    //     _sceneLoader.UnloadBootScene();
    // }
}