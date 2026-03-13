using System;
using System.IO;
using UnityEngine;

public class DocumentManager : MonoBehaviour
{
    private FilePicker filePicker;

    [SerializeField] private SourceManager sourceManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SupabaseDocumentUploader supabaseUploader;
    [SerializeField] private SupabaseSourceService supabaseSourceService;
    [SerializeField] private SupabaseReadService supabaseReadService;

    [Header("Notebook")]
    [SerializeField] private string notebookId = "notebook-001";

    private void Start()
    {
        filePicker = GetComponent<FilePicker>();

        if (filePicker != null)
            filePicker.OnFileImported += OnFileImported;
        else
            Debug.LogError("FilePicker component not found on this GameObject.");

        LoadSources();
    }

    private void OnDestroy()
    {
        if (filePicker != null)
            filePicker.OnFileImported -= OnFileImported;
    }

    private void LoadSources()
    {
        sourceManager.ClearSources();

        supabaseReadService.GetSources(
            notebookId,
            onSuccess: (sources) =>
            {
                if (sources == null)
                    return;

                foreach (var source in sources)
                {
                    string title = Path.GetFileNameWithoutExtension(source.file_name);
                    string description = FormatFileSize(source.file_size);

                    if (!string.IsNullOrEmpty(source.file_type))
                        description += " • " + source.file_type;

                    sourceManager.CreateSource(
                        title,
                        description,
                        () => OnSourceClicked(source.id, title),
                        () => OpenOverviewOptions(source.id)
                    );
                }

                if (sources.Length > 0)
                    uiManager.OpenMainScreenAfterImport();
            },
            onError: (error) =>
            {
                Debug.LogError(error);
            }
        );
    }

    private void OnFileImported(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Debug.LogError("Imported file path is invalid: " + path);
            return;
        }

        string fileName = Path.GetFileName(path);
        string title = Path.GetFileNameWithoutExtension(path);

        FileInfo fileInfo = new FileInfo(path);
        string sizeText = FormatFileSize(fileInfo.Length);
        string dateText = DateTime.Now.ToString("MMM d, yyyy");
        string description = sizeText + " • " + dateText;

        string extension = Path.GetExtension(path).ToLowerInvariant();
        string fileType = GetFileType(extension);
        string mimeType = GetMimeType(extension);

        if (string.IsNullOrEmpty(fileType))
        {
            Debug.LogError("Unsupported file type: " + extension);
            return;
        }

        uiManager.OpenMainScreenAfterImport();

        supabaseUploader.UploadDocument(
            path,
            onSuccess: (fileUrl) =>
            {
                Debug.Log("Upload success: " + fileUrl);

                supabaseSourceService.CreateSourceAndSummarize(
                    fileName,
                    fileUrl,
                    fileInfo.Length,
                    notebookId,
                    fileType,
                    mimeType,
                    onSummaryCreated: (sourceId) =>
                    {
                        Debug.Log("Summary requested for source: " + sourceId);

                        string listDescription = description + " • " + fileType;

                        sourceManager.CreateSource(
                            title,
                            listDescription,
                            () => OnSourceClicked(sourceId, title),
                            () => OpenOverviewOptions(sourceId)
                        );
                    },
                    onError: (error) =>
                    {
                        Debug.LogError("CreateSourceAndSummarize failed: " + error);
                    }
                );
            },
            onError: (error) =>
            {
                Debug.LogError("Upload failed: " + error);
            }
        );
    }

    private void OnSourceClicked(string sourceId, string title)
    {
        supabaseReadService.GetSourceById(
            sourceId,
            onSuccess: (source) =>
            {
                if (source == null)
                {
                    Debug.LogError("Source not found: " + sourceId);
                    return;
                }

                if (source.status == "processing")
                {
                    uiManager.ShowSummary(title, "Summary is being generated...");
                    return;
                }

                if (source.status == "failed")
                {
                    string errorText = string.IsNullOrEmpty(source.error_message)
                        ? "Failed to generate summary."
                        : "Failed to generate summary.\n\n" + source.error_message;

                    uiManager.ShowSummary(title, errorText);
                    return;
                }

                supabaseReadService.GetSummary(
                    sourceId,
                    onSuccess: (summary) =>
                    {
                        uiManager.ShowSummary(title, summary);
                    },
                    onError: (error) =>
                    {
                        Debug.LogError(error);
                    }
                );
            },
            onError: (error) =>
            {
                Debug.LogError(error);
            }
        );
    }

    private void OpenOverviewOptions(string sourceId)
    {
        Debug.Log("Overview options: " + sourceId);
    }

    private string GetFileType(string extension)
    {
        switch (extension)
        {
            case ".pdf":
                return "pdf";

            case ".txt":
                return "text";

            case ".jpg":
            case ".jpeg":
            case ".png":
            case ".webp":
                return "image";

            case ".mp3":
            case ".wav":
            case ".m4a":
            case ".aac":
            case ".ogg":
                return "audio";

            default:
                return null;
        }
    }

    private string GetMimeType(string extension)
    {
        switch (extension)
        {
            case ".pdf":
                return "application/pdf";

            case ".txt":
                return "text/plain";

            case ".jpg":
            case ".jpeg":
                return "image/jpeg";

            case ".png":
                return "image/png";

            case ".webp":
                return "image/webp";

            case ".mp3":
                return "audio/mpeg";

            case ".wav":
                return "audio/wav";

            case ".m4a":
                return "audio/mp4";

            case ".aac":
                return "audio/aac";

            case ".ogg":
                return "audio/ogg";

            default:
                return "application/octet-stream";
        }
    }

    private string FormatFileSize(long bytes)
    {
        if (bytes >= 1024 * 1024)
            return (bytes / (1024f * 1024f)).ToString("F1") + " MB";

        return (bytes / 1024f).ToString("F1") + " KB";
    }
}