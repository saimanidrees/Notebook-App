using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SupabaseReadService : MonoBehaviour
{
    [Header("Supabase")]
    [SerializeField] private string supabaseUrl = "https://YOUR_PROJECT.supabase.co";
    [SerializeField] private string anonKey = "YOUR_ANON_KEY";

    public void GetSources(string notebookId, Action<SourceData[]> onSuccess, Action<string> onError)
    {
        StartCoroutine(GetSourcesRoutine(notebookId, onSuccess, onError));
    }

    public void GetSourceById(string sourceId, Action<SourceData> onSuccess, Action<string> onError)
    {
        StartCoroutine(GetSourceByIdRoutine(sourceId, onSuccess, onError));
    }

    public void GetSummary(string sourceId, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(GetSummaryRoutine(sourceId, onSuccess, onError));
    }

    private IEnumerator GetSourcesRoutine(string notebookId, Action<SourceData[]> onSuccess, Action<string> onError)
    {
        string url = $"{supabaseUrl}/rest/v1/sources?notebook_id=eq.{UnityWebRequest.EscapeURL(notebookId)}&select=*&order=created_at.desc";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("apikey", anonKey);
        request.SetRequestHeader("Authorization", "Bearer " + anonKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke("GetSources failed: " + request.responseCode + "\n" + request.downloadHandler.text);
            yield break;
        }

        SourceData[] rows = JsonHelper.FromJson<SourceData>(request.downloadHandler.text);
        onSuccess?.Invoke(rows);
    }

    private IEnumerator GetSourceByIdRoutine(string sourceId, Action<SourceData> onSuccess, Action<string> onError)
    {
        string url = $"{supabaseUrl}/rest/v1/sources?id=eq.{UnityWebRequest.EscapeURL(sourceId)}&select=*";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("apikey", anonKey);
        request.SetRequestHeader("Authorization", "Bearer " + anonKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke("GetSourceById failed: " + request.responseCode + "\n" + request.downloadHandler.text);
            yield break;
        }

        SourceData[] rows = JsonHelper.FromJson<SourceData>(request.downloadHandler.text);

        if (rows == null || rows.Length == 0)
        {
            onSuccess?.Invoke(null);
            yield break;
        }

        onSuccess?.Invoke(rows[0]);
    }

    private IEnumerator GetSummaryRoutine(string sourceId, Action<string> onSuccess, Action<string> onError)
    {
        string url = $"{supabaseUrl}/rest/v1/source_summaries?source_id=eq.{UnityWebRequest.EscapeURL(sourceId)}&select=*";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("apikey", anonKey);
        request.SetRequestHeader("Authorization", "Bearer " + anonKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke("GetSummary failed: " + request.responseCode + "\n" + request.downloadHandler.text);
            yield break;
        }

        SourceSummaryData[] rows = JsonHelper.FromJson<SourceSummaryData>(request.downloadHandler.text);

        if (rows == null || rows.Length == 0 || string.IsNullOrEmpty(rows[0].summary))
        {
            onSuccess?.Invoke("Summary not ready yet.");
            yield break;
        }

        onSuccess?.Invoke(rows[0].summary);
    }

    [Serializable]
    public class SourceData
    {
        public string id;
        public string file_name;
        public string file_url;
        public long file_size;
        public string created_at;
        public string notebook_id;
        public string file_type;
        public string mime_type;
        public string status;
        public string error_message;
    }

    [Serializable]
    public class SourceSummaryData
    {
        public string id;
        public string source_id;
        public string summary;
        public string created_at;
    }
}