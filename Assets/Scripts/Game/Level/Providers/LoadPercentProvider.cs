using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadPercentProvider
{
    private const float _percentMultiplier = 100f;
    private const int _startPercent = 0;
    private AsyncOperationHandle _operationHandle;
    private bool _isIndicate;
    private bool _isLoadComplete;
    public int GetPercentComplete()
    {
        if (_isIndicate == false )
        {
            return _startPercent;
        }
        else
        {
            if (_isLoadComplete == true)
            {
                return (int)_percentMultiplier;
            }
            else
            {
                return (int)(_operationHandle.PercentComplete * _percentMultiplier);
            }
        }
    }

    protected void SetHandle(AsyncOperationHandle operationHandle)
    {
        _isIndicate = true;
        _operationHandle = operationHandle;
    }
    protected void SetLoadComplete()
    {
        _isLoadComplete = true;
    }
    public void SetDefault()
    {
        _isIndicate = false;
        _isLoadComplete = false;
    }
}