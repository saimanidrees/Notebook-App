using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SourceManager : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject sourcePrefab;
    [SerializeField] private List<SourceButtonUI> sourcesList;
    private Transform sourcesHolder;
    private void Start()
    {
        sourcesHolder = scrollRect.content;
    }
    public void CreateSource(string sourceTitle, string sourceDescription, UnityAction sourceAction, UnityAction overviewOptionsAction)
    {
        GameObject source = Instantiate(sourcePrefab);
        if(sourcesHolder == null)
            sourcesHolder = scrollRect.content;
        source.transform.SetParent(sourcesHolder, false);
        SourceButtonUI sourceButtonUI = source.GetComponent<SourceButtonUI>();
        sourceButtonUI.InitSource(sourceTitle, sourceDescription, sourceAction, overviewOptionsAction);
        sourcesList.Add(sourceButtonUI);
    }
}