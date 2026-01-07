using System.Threading;
using UnityEngine;

public interface ISetLighting
{
    public void SetLightingColor(Color color);
    public void SetLightingColorOnSmoothChangeBackground(Color color, float time, CancellationToken cancellationToken);
}