using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
public class SourceButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText, descriptionText;
    [SerializeField] private Button overviewOptionsBtn;
    public void InitSource(string sourceTitle, string sourceDescription, UnityAction sourceAction, UnityAction overviewOptionsAction)
    {
        SetSourceMetaData(sourceTitle, sourceDescription);
        RegisterSourceBtn(sourceAction);
        RegisterOverviewBtn(overviewOptionsAction);
    }
    private void SetSourceMetaData(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
    }
    private void RegisterSourceBtn(UnityAction method)
    {
        Button sourceBtn = GetComponent<Button>();
        sourceBtn.onClick.RemoveAllListeners();
        sourceBtn.onClick.AddListener(method);
    }
    private void RegisterOverviewBtn(UnityAction method)
    {
        overviewOptionsBtn.onClick.RemoveAllListeners();
        overviewOptionsBtn.onClick.AddListener(method);
    }
}