using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.Networking;

public class DevHelp : MonoBehaviour
{
    public TMP_InputField pathText;
    public TMP_InputField persistentDataPath;
    public TMP_InputField temporaryCachePath;

    public TMP_InputField linkText;
    [Space]
    public bool RoadDebug = false;
    public Movable OB;
    void Start()
    {
        pathText.text = Application.dataPath;
        persistentDataPath.text = Application.persistentDataPath;
        temporaryCachePath.text = Application.temporaryCachePath;
    }

    public void SetRoadsDebug(bool state)
    {
        foreach (var item in FindObjectsOfType<RAI_CrossroadCenter>())
        {
            item.drawDebug = state;
            item.ClearDebug();

            RoadDebug = state;
        }
    }

    public void GetFile()
    {
        string path = linkText.text;

        if (File.Exists(path))
        {
            StreamReader reader = new StreamReader(path);
            RAI_DebugManager.instance.ShowMessage(reader.ReadToEnd(), Color.Lerp(Color.red, Color.yellow, 0.5f));

            reader.Close();
        }
        else
        {
            RAI_DebugManager.instance.ShowMessage("NO FILE", Color.red );
        }
    }

    public void LoadFile()
    {
        string path = linkText.text;

        StartCoroutine(LoadFromServer(path));
    }

    IEnumerator LoadFromServer(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            RAI_DebugManager.instance.ShowMessage("NetworkError", Color.red);
        }
        else
        {
            //Debug.Log(www.downloadHandler.text);
            RAI_DebugManager.instance.ShowMessage(www.downloadHandler.text, Color.Lerp(Color.red, Color.yellow, 0.5f));
            //byte[] results = www.downloadHandler.data;
        }
    }
}
