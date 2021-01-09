using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Logger : MonoBehaviour
{
	public bool openLogDir = false;
	public int showLogSize = 3;
	public string filterString = "";
	private string logSavePath;
	private List<string> logList;
	void Start()
	{
		logList = new List<string>();
		logSavePath = Application.persistentDataPath + "/log";
		if (openLogDir)
			Application.OpenURL(Application.persistentDataPath);
		using (StreamWriter writer = new StreamWriter(logSavePath, true, Encoding.UTF8))
		{
			writer.WriteLine("\n\n----------------------------- 日志分隔线 -----------------------------");
			writer.WriteLine("----------------------------- " + System.DateTime.Now + " -----------------------------");
		}
		//注册日志处理函数
		Application.logMessageReceived += HandleLog;
	}

	void HandleLog(string logString, string stackTrace, LogType type)
	{
		//将所有日志写入到日志文件
		using (StreamWriter writer = new StreamWriter(logSavePath, true, Encoding.UTF8))
		{
			writer.WriteLine(logString + "\n\t\t" + type + ": " + stackTrace.Replace("\n", "\n\t\t"));
		}
		//设置过滤条件，将指定类型、包含某些字符串的日志保存到屏幕日志窗器中
		bool show = false;
		//置过滤条件:指定类型
		if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
		{
			show = true;
		}
		//置过滤条件:包含指定关键字，多个关键字用 | 隔开
		foreach (string str in filterString.Split('|'))
		{
			if (logString.Contains(str))
			{
				show = true;
				break;
			}
		}
		if (show)
		{
			logList.Add(logString);
			if (logList.Count > showLogSize)
			{
				logList.RemoveAt(0);
			}
		}
	}
}
