using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugToScreen : MonoBehaviour
{
	string myLog;
	Queue<string> myLogQueue = new Queue<string>();
	GUIStyle logStyle;
	Vector2 scrollPosition;
	int maxLogs = 50; // 로그의 최대 수
	Rect logWindowRect; // 로그 창의 위치와 크기를 저장하는 Rect 변수

	void Start()
	{
		// GUIStyle 설정
		logStyle = new GUIStyle
		{
			fontSize = 25 // 글꼴 크기 설정
		};
		logStyle.normal.textColor = Color.red; // 글자 색상 설정

		// 로그 창의 크기와 위치 설정
		float logHeight = Screen.height / 3;
		logWindowRect = new Rect(0, Screen.height - logHeight, Screen.width, logHeight);
	}

	void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}

	void HandleLog(string logString, string stackTrace, LogType type)
	{
		string newString = "\n [" + type + "] : " + logString;
		myLogQueue.Enqueue(newString);
		if (type == LogType.Exception)
		{
			newString = "\n" + stackTrace;
			myLogQueue.Enqueue(newString);
		}
		// 로그의 최대 수를 넘으면 오래된 로그를 제거
		if (myLogQueue.Count > maxLogs)
		{
			myLogQueue.Dequeue();
		}

		myLog = string.Empty;
		foreach (string log in myLogQueue)
		{
			myLog += log;
		}

		// 최신 로그로 자동 스크롤
		scrollPosition.y = float.MaxValue;
	}

	void OnGUI()
	{
		// 로그 창의 위치와 크기 설정
		GUILayout.BeginArea(logWindowRect);

		// 스크롤 가능한 영역 시작
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(logWindowRect.width), GUILayout.Height(logWindowRect.height));
		GUILayout.Label(myLog, logStyle);
		GUILayout.EndScrollView(); // 스크롤 가능한 영역 끝

		GUILayout.EndArea();
	}
}
