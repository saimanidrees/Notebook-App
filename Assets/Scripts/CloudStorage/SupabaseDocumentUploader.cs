using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class SupabaseDocumentUploader : MonoBehaviour
{
    [Header("Supabase")]
    [SerializeField] private string supabaseUrl = "https://YOUR_PROJECT.supabase.co";
    [SerializeField] private string anonKey = "YOUR_ANON_KEY";
    [SerializeField] private string bucketName = "documents";

    public void UploadDocument(string localFilePath, Action<string> onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(UploadRoutine(localFilePath, onSuccess, onError));
    }

    private IEnumerator UploadRoutine(string localFilePath, Action<string> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(localFilePath) || !File.Exists(localFilePath))
        {
            onError?.Invoke("File not found.");
            yield break;
        }

        byte[] fileBytes;
        try
        {
            fileBytes = File.ReadAllBytes(localFilePath);
        }
        catch (Exception e)
        {
            onError?.Invoke("Failed to read file: " + e.Message);
            yield break;
        }

        string fileName = Path.GetFileName(localFilePath);
        string remotePath = $"uploads/{DateTime.UtcNow:yyyyMMdd_HHmmss}_{fileName}";
        string uploadUrl = $"{supabaseUrl}/storage/v1/object/{bucketName}/{remotePath}";

        UnityWebRequest request = new UnityWebRequest(uploadUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(fileBytes);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("apikey", anonKey);
        request.SetRequestHeader("Authorization", "Bearer " + anonKey);
        request.SetRequestHeader("Content-Type", GetMimeType(localFilePath));
        request.SetRequestHeader("x-upsert", "false");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke($"Upload failed: {request.responseCode}\n{request.downloadHandler.text}");
            yield break;
        }

        string publicUrl = $"{supabaseUrl}/storage/v1/object/public/{bucketName}/{remotePath}";
        onSuccess?.Invoke(publicUrl);
    }

    private string GetMimeType(string filePath)
    {
        string ext = Path.GetExtension(filePath).ToLowerInvariant();
        switch (ext)
        {
            case ".pdf": return "application/pdf";
            case ".txt": return "text/plain";
            default: return "application/octet-stream";
        }
    }
}