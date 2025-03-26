using UnityEngine;


public class OpenUrl : MonoBehaviour
{
    public string Url;
    public void GoToSite()
    {
        Application.OpenURL(Url);
    }
}