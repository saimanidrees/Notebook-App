using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SourceManager : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject sourcePrefab;
    [SerializeField] private List<SourceButtonUI> sourcesList = new List<SourceButtonUI>();

    private Transform sourcesHolder;

    private void Start()
    {
        if (scrollRect != null)
            sourcesHolder = scrollRect.content;
    }

    public void CreateSource(string sourceTitle, string sourceDescription, UnityAction sourceAction, UnityAction overviewOptionsAction)
    {
        if (sourcesHolder == null && scrollRect != null)
            sourcesHolder = scrollRect.content;

        GameObject source = Instantiate(sourcePrefab, sourcesHolder, false);

        SourceButtonUI sourceButtonUI = source.GetComponent<SourceButtonUI>();
        if (sourceButtonUI != null)
        {
            sourceButtonUI.InitSource(sourceTitle, sourceDescription, sourceAction, overviewOptionsAction);
            sourcesList.Add(sourceButtonUI);
        }
        else
        {
            Debug.LogError("SourceButtonUI component missing on sourcePrefab.");
        }
    }

    public void ClearSources()
    {
        if (sourcesHolder == null && scrollRect != null)
            sourcesHolder = scrollRect.content;

        if (sourcesHolder == null)
            return;

        for (int i = sourcesHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(sourcesHolder.GetChild(i).gameObject);
        }

        sourcesList.Clear();
    }
}