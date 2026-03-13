using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SummaryScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI summaryTitleText;
    [SerializeField] private TextMeshProUGUI summaryBodyText;
    public void ShowSummary(string title, string summary)
    {
        if (summaryTitleText != null)
            summaryTitleText.text = title;
        if (summaryBodyText != null)
            summaryBodyText.text = summary;
    }
}
