using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private MainScreen mainScreen;
    [SerializeField] private CreationScreen creationScreen;
    [Header("Main Screen Elements")]
    [SerializeField] private GameObject letsGetStarted;
    [SerializeField] private ScrollRect sourcesScrollRect;
    public void OpenCreationScreen()
    {
        mainScreen.gameObject.SetActive(false);
        creationScreen.gameObject.SetActive(true);
    }
    public void OpenMainScreenAfterImport()
    {
        creationScreen.gameObject.SetActive(false);
        mainScreen.gameObject.SetActive(true);
        if (letsGetStarted != null)
            letsGetStarted.SetActive(false);
        if (sourcesScrollRect != null)
            sourcesScrollRect.gameObject.SetActive(true);
    }
}