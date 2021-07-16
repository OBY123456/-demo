using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ConfigData
{
    public int 定时调用GC;

    public bool 是否显示鼠标;

    public bool 是否开启软件前置;

    public string 去水印码;

    public float 大图自动消失时间;

    public float 手指滑动灵敏度;

    public float 大图飞行速度;

    public float 大图出现动画时间;

    public float 小图移动速度;

    public float 小图行间隔;

    public float 小图列间隔;
}


public class Config : MonoBehaviour
{
    public static Config Instance;

    public ConfigData configData  = new ConfigData();

    private string File_name = "config.txt";
    private string Path;

    private void Awake()
    {
        Instance = this;
        configData = new ConfigData();
#if UNITY_STANDALONE_WIN
        Path = Application.streamingAssetsPath + "/" + File_name;
        if (FileHandle.Instance.IsExistFile(Path))
        {
            string st = FileHandle.Instance.FileToString(Path);
            LogMsg.Instance.Log(st);
            configData = JsonConvert.DeserializeObject<ConfigData>(st);
        }
#elif UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
        Path = Application.persistentDataPath + "/" + File_name;
        if(FileHandle.Instance.IsExistFile(Path))
        {
            string st = FileHandle.Instance.FileToString(Path);
            configData = JsonConvert.DeserializeObject<ConfigData>(st);
        }
        else
        {
            Path = Application.streamingAssetsPath + "/" + File_name;
            if (FileHandle.Instance.IsExistFile(Path))
            {
                string st = FileHandle.Instance.FileToString(Path);
                configData = JsonConvert.DeserializeObject<ConfigData>(st);
            }
        }
#endif
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        SaveData();
#endif
    }

    public void SaveData()
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
         Path = Application.persistentDataPath + "/" + File_name;
#endif
        string st = JsonConvert.SerializeObject(configData);
        FileHandle.Instance.SaveFile(st, Path);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
