using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private MainScreen mainScreen;
    [SerializeField] private CreationScreen creationScreen;
    [SerializeField] private SummaryScreen summaryPanel;

    [Header("Main Screen Elements")]
    [SerializeField] private GameObject letsGetStarted;
    [SerializeField] private ScrollRect sourcesScrollRect;

    public void OpenCreationScreen()
    {
        if (mainScreen != null)
            mainScreen.gameObject.SetActive(false);

        if (creationScreen != null)
            creationScreen.gameObject.SetActive(true);
    }

    public void OpenMainScreenAfterImport()
    {
        if (creationScreen != null)
            creationScreen.gameObject.SetActive(false);

        if (mainScreen != null)
            mainScreen.gameObject.SetActive(true);

        if (letsGetStarted != null)
            letsGetStarted.SetActive(false);

        if (sourcesScrollRect != null)
            sourcesScrollRect.gameObject.SetActive(true);
    }

    public void ShowSummary(string title, string summary)
    {
        if (summaryPanel != null)
            summaryPanel.gameObject.SetActive(true);

        if (mainScreen != null)
            mainScreen.gameObject.SetActive(false);

        if (summaryPanel != null)
            summaryPanel.ShowSummary(title, summary);
    }
}