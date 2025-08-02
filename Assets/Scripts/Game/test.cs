
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class test : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public int count;
    public string t;

    private string space = "\u00A0\u00A0";
    private int value;
    private TextConsistentlyViewer _textConsistentlyViewer;
    private CancellationTokenSource _cancellationToken;
    [Button()]
    private void stop()
    {
        _cancellationToken?.Cancel();
    }
    
    [Button()]
    private void add2()
    {
        _textConsistentlyViewer = new TextConsistentlyViewer(Text);
        _cancellationToken = new CancellationTokenSource();
        _textConsistentlyViewer.ClearText();
        _textConsistentlyViewer.SetTextConsistently(_cancellationToken.Token, t).Forget();
    }
}