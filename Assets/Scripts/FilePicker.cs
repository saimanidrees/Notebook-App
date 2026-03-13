using System;
using System.IO;
using UnityEngine;

public class FilePicker : MonoBehaviour
{
    public Action<string> OnFileImported;

    private string pdfType;
    private string txtType;
    private string jpgType;
    private string jpegType;
    private string pngType;
    private string webpType;
    private string mp3Type;
    private string wavType;
    private string m4aType;
    private string aacType;
    private string oggType;

    private const string Pdf = "pdf";
    private const string Txt = "txt";
    private const string Jpg = "jpg";
    private const string Jpeg = "jpeg";
    private const string Png = "png";
    private const string Webp = "webp";
    private const string Mp3 = "mp3";
    private const string Wav = "wav";
    private const string M4a = "m4a";
    private const string Aac = "aac";
    private const string Ogg = "ogg";

    private void Start()
    {
        pdfType = NativeFilePicker.ConvertExtensionToFileType(Pdf);
        txtType = NativeFilePicker.ConvertExtensionToFileType(Txt);
        jpgType = NativeFilePicker.ConvertExtensionToFileType(Jpg);
        jpegType = NativeFilePicker.ConvertExtensionToFileType(Jpeg);
        pngType = NativeFilePicker.ConvertExtensionToFileType(Png);
        webpType = NativeFilePicker.ConvertExtensionToFileType(Webp);
        mp3Type = NativeFilePicker.ConvertExtensionToFileType(Mp3);
        wavType = NativeFilePicker.ConvertExtensionToFileType(Wav);
        m4aType = NativeFilePicker.ConvertExtensionToFileType(M4a);
        aacType = NativeFilePicker.ConvertExtensionToFileType(Aac);
        oggType = NativeFilePicker.ConvertExtensionToFileType(Ogg);

        Debug.Log("pdfType: " + pdfType);
        Debug.Log("txtType: " + txtType);
        Debug.Log("jpgType: " + jpgType);
        Debug.Log("jpegType: " + jpegType);
        Debug.Log("pngType: " + pngType);
        Debug.Log("webpType: " + webpType);
        Debug.Log("mp3Type: " + mp3Type);
        Debug.Log("wavType: " + wavType);
        Debug.Log("m4aType: " + m4aType);
        Debug.Log("aacType: " + aacType);
        Debug.Log("oggType: " + oggType);
    }

    public void PickDocument()
    {
        if (NativeFilePicker.IsFilePickerBusy())
            return;

        NativeFilePicker.PickFile(OnFilePicked, new string[] { pdfType, txtType });
    }

    public void PickImage()
    {
        if (NativeFilePicker.IsFilePickerBusy())
            return;

        NativeFilePicker.PickFile(OnFilePicked, new string[] { jpgType, jpegType, pngType, webpType });
    }

    public void PickAudio()
    {
        if (NativeFilePicker.IsFilePickerBusy())
            return;

        NativeFilePicker.PickFile(OnFilePicked, new string[] { mp3Type, wavType, m4aType, aacType, oggType });
    }

    public void PickAnySupportedFile()
    {
        if (NativeFilePicker.IsFilePickerBusy())
            return;

        NativeFilePicker.PickFile(
            OnFilePicked,
            new string[]
            {
                pdfType, txtType,
                jpgType, jpegType, pngType, webpType,
                mp3Type, wavType, m4aType, aacType, oggType
            }
        );
    }

    private void OnFilePicked(string path)
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
            string newPath = Path.Combine(Application.persistentDataPath, fileName);

            File.Copy(path, newPath, true);

            Debug.Log("File copied to: " + newPath);
            OnFileImported?.Invoke(newPath);
        }
        catch (Exception e)
        {
            Debug.LogError("File import failed: " + e.Message);
        }
    }
}