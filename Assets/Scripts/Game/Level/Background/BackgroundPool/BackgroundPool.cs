using UnityEngine;

public class BackgroundPool
{
    private const int _count = 2;
    private readonly GameObject _prefab;
    private readonly Transform _parent;
    private readonly PoolBase<SpriteRenderer> _pool;
    public BackgroundPool(GameObject prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
        _pool = new PoolBase<SpriteRenderer>(OnCreate, null, OnReturn, _count);
    }
    public SpriteRenderer GetRenderer()
    {
        return _pool.Get();
    }
    
    private SpriteRenderer OnCreate()
    {
        var res = Object.Instantiate(_prefab, _parent);
        return res.GetComponent<SpriteRenderer>();
    }
    private void OnReturn(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.gameObject.SetActive(false);
    }
}