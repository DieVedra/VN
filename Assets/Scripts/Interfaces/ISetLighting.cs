using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISetLighting
{
    public void ChangeLightingColorOfTheCharacter(Color color);
    public UniTask SmoothChangeLightingColorOfTheCharacter(Color color, float time, CancellationToken cancellationToken);
}