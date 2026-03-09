using System.IO;
using UnityEngine;
using System;

public class DocumentManager : MonoBehaviour
{
    private FilePicker filePicker;

    [SerializeField] private SourceManager sourceManager;
    [SerializeField] private UIManager uiManager;

    private void Start()
    {
        filePicker = GetComponent<FilePicker>();
        filePicker.OnFileImported += OnFileImported;
    }

    private void OnFileImported(string path)
    {
        Debug.Log("File imported: " + path);

        string fileName = Path.GetFileNameWithoutExtension(path);

        FileInfo fileInfo = new FileInfo(path);
        string sizeText = FormatFileSize(fileInfo.Length);
        string dateText = DateTime.Now.ToString("MMM d, yyyy");

        string description = sizeText + " • " + dateText;

        sourceManager.CreateSource(
            fileName,
            description,
            () => OpenSource(path),
            () => OpenOverviewOptions(path)
        );

        // 🔹 Update UI
        uiManager.OpenMainScreenAfterImport();
    }

    void OpenSource(string path)
    {
        Debug.Log("Open source file: " + path);
    }

    void OpenOverviewOptions(string path)
    {
        Debug.Log("Overview options for: " + path);
    }

    string FormatFileSize(long bytes)
    {
        if (bytes >= 1024 * 1024)
            return (bytes / (1024f * 1024f)).ToString("F1") + " MB";
        else
            return (bytes / 1024f).ToString("F1") + " KB";
    }
}