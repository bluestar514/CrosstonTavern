using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class FileHandler
{
    public string fileName;
    public string folder;

    string seperator;

    string errorMsg;

    bool displayedError = false;

    public FileHandler(string fileName, string folder)
    {
        this.fileName = fileName;
        this.folder = folder;

        seperator = "/";

        errorMsg = "Logging for " + GetPath() + " does not seem to be working. We likely do not have write permissions.";
    }

    public void NewFile()
    {
        try {
            if(!Directory.Exists(GetFolderPath()))
                Directory.CreateDirectory(GetFolderPath());

            StreamWriter writer = new StreamWriter(GetPath(), false);

            System.DateTime time = System.DateTime.Now;
            writer.WriteLine(time.ToString());

            writer.Close();
        } catch {
            if(!displayedError)
                Debug.LogError(errorMsg);

            displayedError = true;
        }

    }

    public void WriteString(string printedText)
    {
        try {
            StreamWriter writer = new StreamWriter(GetPath(), true);
            writer.WriteLine(printedText);
            writer.Close();
        } catch {
            if (!displayedError)
                Debug.LogError(errorMsg);
            displayedError = true;
        }
    }

    public string GetPath()
    {
        string path = "";

#if UNITY_EDITOR
        path = GetPathEditor();
#else
        path = GetPathRunTime();
#endif
        return path;
    }

    public string GetFolderPath()
    {
        string path = "";

#if UNITY_EDITOR
        path = GetFolderPathEditor();
#else
        path = GetFolderPathRunTime();
#endif
        return path;
    }


    string GetPathEditor()
    {
        return string.Join(seperator, new List<string>() { GetFolderPath(), fileName+ ".txt"});
    }

    string GetPathRunTime()
    {
        return string.Join(seperator, new List<string>() { GetFolderPath(), fileName+ ".txt" });
    }

    string GetFolderPathEditor()
    {
        return string.Join(seperator, new List<string>() { "Assets/Resources", folder});
    }

    string GetFolderPathRunTime()
    {
        return string.Join(seperator, new List<string>() { Application.persistentDataPath, folder});
    }
}
