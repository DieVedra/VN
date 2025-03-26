using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPoints : MonoBehaviour
{
    [SerializeField] private Transform _pointLeft;
    [SerializeField] private Transform _pointCenter;
    [SerializeField] private Transform _pointRight;

    public Transform PointLeft => _pointLeft;
    public Transform PointCenter => _pointCenter;
    public Transform PointRight => _pointRight;
}
