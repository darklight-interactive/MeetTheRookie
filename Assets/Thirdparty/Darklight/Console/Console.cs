using System;
using System.Collections.Generic;

using UnityEngine;

using Darklight.UnityExt.CustomEditor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Console
{
	public enum LogSeverity { Info, Warning, Error }

	public class ConsoleGUI
	{
		private Vector2 scrollPosition;
		private bool autoScroll = true; // Default to true to enable auto-scrolling.
		public class LogEntry
		{
			DateTime _timeStamp = DateTime.Now;
			LogSeverity _severity = LogSeverity.Info;
			string _message = string.Empty;
			int _indentLevel = 0;

			public string Timestamp => _timeStamp.ToString("mm:ss:ff");
			public string Message => new string(' ', _indentLevel * 4) + $"{_message}";
			public GUIStyle Style
			{
				get
				{
					Color textColor = Color.white; // Default to white
					switch (_severity)
					{
						case LogSeverity.Warning:
							textColor = Color.yellow;
							break;
						case LogSeverity.Error:
							textColor = Color.red;
							break;
					}

					return new GUIStyle(GUI.skin.label)
					{
						normal = { textColor = textColor },
						alignment = TextAnchor.MiddleLeft
					};
				}
			}
			public LogEntry(int indentLevel, string message, LogSeverity severity = LogSeverity.Info)
			{
				_indentLevel = indentLevel;
				_message = message;
				_severity = severity;
				_timeStamp = DateTime.Now;
			}
		}

		public List<LogEntry> AllLogEntries { get; private set; } = new List<LogEntry>();

		public void Log(string message, int indent = 0, LogSeverity severity = LogSeverity.Info)
		{
			LogEntry newLog = new LogEntry(indent, message, severity);
			AllLogEntries.Add(newLog);

			// If autoScroll is enabled, adjust the scroll position to the bottom.
			if (autoScroll)
			{
				// Assuming each log entry is roughly the same height, this will scroll to the bottom.
				scrollPosition.y = float.MaxValue;
			}
		}

		public void Reset()
		{
			AllLogEntries.Clear();
		}

#if UNITY_EDITOR
		public void DrawInEditor()
		{
			// Toggle for enabling/disabling auto-scroll
			autoScroll = EditorGUILayout.Toggle("Auto-scroll", autoScroll);

			// Dark gray background
			GUIStyle backgroundStyle = new GUIStyle
			{
				normal = { background = CustomInspectorGUI.MakeTex(600, 1, new Color(0.1f, 0.1f, 0.1f, 1.0f)) }
			};

			// Creating a scroll view with a custom background
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, backgroundStyle, GUILayout.Height(200));

			int logCount = 0;
			foreach (LogEntry log in AllLogEntries)
			{
				EditorGUILayout.BeginHorizontal(); // Start a horizontal group for inline elements

				string message = $"[{log.Timestamp}] || {logCount} || {log.Message}";
				EditorGUILayout.LabelField(message, CustomGUIStyles.SmallTextStyle, GUILayout.ExpandWidth(true));
				EditorGUILayout.EndHorizontal();

				/*
				// Divider Line
				EditorGUILayout.BeginHorizontal();
				GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
				EditorGUILayout.EndHorizontal();
				*/

				logCount++;
			}
			EditorGUILayout.EndScrollView();
		}
#endif

	}
}