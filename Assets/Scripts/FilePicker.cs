using System;
using System.IO;
using UnityEngine;

public class FilePicker : MonoBehaviour
{
    public Action<string> OnFileImported;
    private string pdfType;
    private string txtType;
    private const string Pdf = "pdf", Txt = "txt";
    void Start()
    {
        // Convert extensions to correct MIME/UTI automatically
        pdfType = NativeFilePicker.ConvertExtensionToFileType(Pdf);
        txtType = NativeFilePicker.ConvertExtensionToFileType(Txt);
    }
    public void PickFile()
    {
        // Prevent opening picker multiple times
        if (NativeFilePicker.IsFilePickerBusy())
            return;

        NativeFilePicker.PickFile((path) =>
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("File selection cancelled");
                return;
            }

            Debug.Log("Picked file: " + path);

            try
            {
                string fileName = Path.GetFileName(path);

                // Copy file to persistentDataPath
                string newPath = Path.Combine(Application.persistentDataPath, fileName);

                File.Copy(path, newPath, true);

                Debug.Log("File copied to: " + newPath);

                OnFileImported?.Invoke(newPath);
            }
            catch (Exception e)
            {
                Debug.LogError("File import failed: " + e.Message);
            }

        }, new string[] { pdfType, txtType });
    }
}