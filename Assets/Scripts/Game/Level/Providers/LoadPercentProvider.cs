
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadPercentProvider
{
    private const float _percentMultiplier = 100f;
    private const int _startPercent = 0;
    private AsyncOperationHandle _operationHandle;


    private bool _isIndicate;


    public int GetPercentComplete()
    {
        if (_isIndicate == false )
        {
            return _startPercent;
        }
        else
        {
            return (int)(_operationHandle.PercentComplete * _percentMultiplier);
        }
    }

    protected void SetHandle(AsyncOperationHandle operationHandle)
    {
        _isIndicate = true;
        _operationHandle = operationHandle;
    }

    public void SkipIndicate()
    {
        _isIndicate = false;
    }
}