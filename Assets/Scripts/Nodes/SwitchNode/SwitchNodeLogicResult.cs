
public struct SwitchNodeLogicResult
{
    public readonly bool CaseFoundSuccessfuly;
    public readonly int IndexCase;

    public SwitchNodeLogicResult(bool caseFoundSuccessfuly = false, int indexCase = 0)
    {
        CaseFoundSuccessfuly = caseFoundSuccessfuly;
        IndexCase = indexCase;
    }
}