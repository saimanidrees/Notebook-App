using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SupabaseSourceService : MonoBehaviour
{
    [Header("Supabase")]
    [SerializeField] private string supabaseUrl = "https://YOUR_PROJECT.supabase.co";
    [SerializeField] private string anonKey = "YOUR_ANON_KEY";

    public void CreateSourceAndSummarize(
        string fileName,
        string fileUrl,
        long fileSize,
        string notebookId,
        string fileType,
        string mimeType,
        Action<string> onSummaryCreated = null,
        Action<string> onError = null)
    {
        StartCoroutine(CreateSourceAndSummarizeRoutine(
            fileName,
            fileUrl,
            fileSize,
            notebookId,
            fileType,
            mimeType,
            onSummaryCreated,
            onError
        ));
    }

    private IEnumerator CreateSourceAndSummarizeRoutine(
        string fileName,
        string fileUrl,
        long fileSize,
        string notebookId,
        string fileType,
        string mimeType,
        Action<string> onSummaryCreated,
        Action<string> onError)
    {
        string sourceId = null;

        yield return StartCoroutine(InsertSource(
            fileName,
            fileUrl,
            fileSize,
            notebookId,
            fileType,
            mimeType,
            id => sourceId = id,
            onError
        ));

        if (string.IsNullOrEmpty(sourceId))
            yield break;

        yield return StartCoroutine(CallSummarizeFunction(
            sourceId,
            fileUrl,
            fileName,
            fileType,
            mimeType,
            () => onSummaryCreated?.Invoke(sourceId),
            onError
        ));
    }

    private IEnumerator InsertSource(
        string fileName,
        string fileUrl,
        long fileSize,
        string notebookId,
        string fileType,
        string mimeType,
        Action<string> onSuccess,
        Action<string> onError)
    {
        string url = $"{supabaseUrl}/rest/v1/sources";

        string json = JsonUtility.ToJson(new SourceInsertRequest
        {
            file_name = fileName,
            file_url = fileUrl,
            file_size = fileSize,
            notebook_id = notebookId,
            file_type = fileType,
            mime_type = mimeType,
            status = "processing",
            error_message = null
        });

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("apikey", anonKey);
        request.SetRequestHeader("Authorization", "Bearer " + anonKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Prefer", "return=representation");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke("Insert source failed: " + request.responseCode + "\n" + request.downloadHandler.text);
            yield break;
        }

        Debug.Log("Insert source response: " + request.downloadHandler.text);

        SourceRow[] rows = JsonHelper.FromJson<SourceRow>(request.downloadHandler.text);
        if (rows == null || rows.Length == 0 || string.IsNullOrEmpty(rows[0].id))
        {
            onError?.Invoke("Insert source succeeded but source id not found.");
            yield break;
        }

        onSuccess?.Invoke(rows[0].id);
    }

    private IEnumerator CallSummarizeFunction(
        string sourceId,
        string fileUrl,
        string fileName,
        string fileType,
        string mimeType,
        Action onSuccess,
        Action<string> onError)
    {
        string url = $"{supabaseUrl}/functions/v1/summarize-document";

        string json = JsonUtility.ToJson(new SummarizeRequest
        {
            source_id = sourceId,
            file_url = fileUrl,
            file_name = fileName,
            file_type = fileType,
            mime_type = mimeType
        });

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("apikey", anonKey);
        request.SetRequestHeader("Authorization", "Bearer " + anonKey);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke("Summarize function failed: " + request.responseCode + "\n" + request.downloadHandler.text);
            yield break;
        }

        Debug.Log("Summarize function response: " + request.downloadHandler.text);
        onSuccess?.Invoke();
    }

    [Serializable]
    private class SourceInsertRequest
    {
        public string file_name;
        public string file_url;
        public long file_size;
        public string notebook_id;
        public string file_type;
        public string mime_type;
        public string status;
        public string error_message;
    }

    [Serializable]
    private class SummarizeRequest
    {
        public string source_id;
        public string file_url;
        public string file_name;
        public string file_type;
        public string mime_type;
    }

    [Serializable]
    private class SourceRow
    {
        public string id;
        public string file_name;
        public string file_url;
        public long file_size;
        public string notebook_id;
        public string file_type;
        public string mime_type;
        public string status;
        public string error_message;
    }
}