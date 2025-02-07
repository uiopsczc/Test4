#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace CsCat
{
	// Begin a vertical group and get its rect back.
	public class EditorGUILayoutBeginVerticalIndentLevelScope : IDisposable
	{
		public const float Width_Of_Per_Indent_Level = 10f;
		public static int Global_Indent_Level;
		private int _add;

		public void _Init(int add = 1)
		{
			this._add = add;
			Global_Indent_Level += add;
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(Global_Indent_Level * Width_Of_Per_Indent_Level);
			//      GUILayout.FlexibleSpace();
		}

		public EditorGUILayoutBeginVerticalIndentLevelScope(int add = 1, params GUILayoutOption[] options)
		{
			_Init(add);
			EditorGUILayout.BeginVertical(options);
		}

		public EditorGUILayoutBeginVerticalIndentLevelScope(GUIStyle style, int add = 1,
			params GUILayoutOption[] options)
		{
			_Init(add);
			EditorGUILayout.BeginVertical(style, options);
		}

		public EditorGUILayoutBeginVerticalIndentLevelScope(ref Rect rect, int add = 1,
			params GUILayoutOption[] options)
		{
			_Init(add);
			rect = EditorGUILayout.BeginVertical(options);
		}

		public EditorGUILayoutBeginVerticalIndentLevelScope(ref Rect rect, GUIStyle style, int add = 1,
			params GUILayoutOption[] options)
		{
			_Init(add);
			rect = EditorGUILayout.BeginVertical(style, options);
		}

		public void Dispose()
		{
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			Global_Indent_Level -= _add;
		}
	}
}
#endif