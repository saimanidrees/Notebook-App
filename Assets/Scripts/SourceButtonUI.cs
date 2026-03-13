using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class SourceButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button overviewOptionsBtn;

    public void InitSource(string sourceTitle, string sourceDescription, UnityAction sourceAction, UnityAction overviewOptionsAction)
    {
        SetSourceMetaData(sourceTitle, sourceDescription);
        RegisterSourceBtn(sourceAction);
        RegisterOverviewBtn(overviewOptionsAction);
    }

    private void SetSourceMetaData(string title, string description)
    {
        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;
    }

    private void RegisterSourceBtn(UnityAction method)
    {
        Button sourceBtn = GetComponent<Button>();
        if (sourceBtn == null)
        {
            Debug.LogError("Button component missing on SourceButtonUI object.");
            return;
        }

        sourceBtn.onClick.RemoveAllListeners();

        if (method != null)
            sourceBtn.onClick.AddListener(method);
    }

    private void RegisterOverviewBtn(UnityAction method)
    {
        if (overviewOptionsBtn == null)
        {
            Debug.LogError("Overview options button is not assigned.");
            return;
        }

        overviewOptionsBtn.onClick.RemoveAllListeners();

        if (method != null)
            overviewOptionsBtn.onClick.AddListener(method);
    }
}