using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using Tayx.Graphy;

public class DataLogger : MonoBehaviour
{
    public delegate void AddToFileLogDelegate(float fps, float avgFps, float ram, float mem);
    public static AddToFileLogDelegate AddToFileLog;

    [SerializeField] private GraphyManager graphy;

    public static string LogPath;
    public static DataLogger instance;

    private StreamWriter sw;

    private string lastFilePath = "";

    private Dictionary<string, int> counterDict = new Dictionary<string, int>();


    private void Awake()
    {
        LogPath = $"{Application.persistentDataPath}/Logs/";
        if (instance == null)
        {
            instance = this;

            AddToFileLog = AddToLog;

#if UNITY_EDITOR
            EditorApplication.quitting += EditorApplication_quitting;
#endif 

            transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);

            CreateWriters();
        }
        else
        {
            Destroy(this);
            return;
        }

    }

    float t = 5f;
    private void Update()
    {
        if (t >= 5f)
        {
            t = 0;
            AddToLog(graphy.CurrentFPS, graphy.AverageFPS, graphy.ReservedRam, graphy.AllocatedRam);
        }

        t += Time.deltaTime;
    }

    private void CreateWriters()
    {
        if (sw == null)
        {
            CreateWriter(LogPath, ref sw, "Log", ".csv", new string[] {
                "Time, Game Time, FPS, Avg FPS, Reserved RAM, Alloc Ram"
                });
        }
    }

    public void AddToLog(float fps, float avgFps, float ram, float mem)
    {
        string time = DateTime.Now.ToString("HH:mm:ss");
        AddToLog(sw, $"{time}, {Time.time}, {fps.ToString("F")}, {avgFps.ToString("F")}, {ram.ToString("F")}, {mem.ToString("F")}");
    }

    private void CreateWriter(string FilePath, ref StreamWriter writer, string logfileName = "Log", string fileExt = ".txt", params string[] addData)
    {
        if (!Directory.Exists(FilePath))
        {
            Directory.CreateDirectory(FilePath);
        }
        string datetime = DateTime.Now.ToString("(yyyy-MM-dd) (HH-mm)");
        string filePath = Path.Combine(FilePath, $"{logfileName} {datetime}{fileExt}");
        string basePath = filePath;

        if (!counterDict.ContainsKey(basePath))
        {
            counterDict.Add(basePath, 1);
        }

        if (File.Exists(filePath))
        {
            while (File.Exists(filePath))
            {
                filePath = Path.Combine(FilePath, $"{logfileName} {datetime} ({counterDict[basePath]}){fileExt}");
                counterDict[basePath]++;
            }
        }
        Debug.Log("Creating log file at: " + filePath);
        FileStream stream = File.Create(filePath);

        writer = new StreamWriter(stream);

        if (!lastFilePath.Equals(filePath))
        {
            counterDict[basePath] = 1;
            lastFilePath = filePath;
        }

        if (addData != null)
        {
            for (int i = 0; i < addData.Length; i++)
            {
                AddToLog(writer, addData[i]);
            }
        }
    }

    public void AddToLog(StreamWriter writer, string logInfo = "")
    {
        if (writer == null)
        {
            return;
        }

        if (logInfo.Length == 0)
        {
            writer.WriteLineAsync();
            return;
        }
        writer.WriteLineAsync(logInfo);
    }

    private void OnApplicationQuit()
    {
        SaveLog();
        CloseLogs();
    }

    private void EditorApplication_quitting()
    {
        SaveLog();
        CloseLogs();
    }

    void SaveLog()
    {
        Debug.Log("CLOSED DEBUG FILE");
        if (sw != null)
        {
            sw.Flush();
        }
    }

    void CloseLogs()
    {
        if (sw != null) { sw.Close(); }
    }
}
