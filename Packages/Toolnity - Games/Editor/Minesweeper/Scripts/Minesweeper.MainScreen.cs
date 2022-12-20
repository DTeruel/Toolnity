#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity.Games
{
	public partial class Minesweeper
	{
		private const string EDITOR_PREF_NICKNAME_ID = "/Toolnity/Minesweeper/Nickname";
		
		private static string nickName;
		private static bool nickNameChecked;
		
		private static void OnEnterMainScreenState()
		{
			nickName = EditorPrefs.GetString(Application.productName + EDITOR_PREF_NICKNAME_ID, string.Empty);
		}
		
		private static void DrawMainScreenState()
		{
			DrawNickName();
			GUILayout.Space(20);
			DrawStartButton();
		}
		
		private static void DrawNickName()
		{
			if (!nickNameChecked)
			{
				OnEnterMainScreenState();
				nickNameChecked = true;
			}
			
			GUILayout.Label("Nickname:");
			var newNickname = GUILayout.TextField(nickName, GUILayout.Width(200f));
			if (nickName != newNickname)
			{
				nickName = newNickname;
				EditorPrefs.SetString(Application.productName + EDITOR_PREF_NICKNAME_ID, nickName);
			}
		}
		
		private static void DrawStartButton()
		{
			EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(nickName));
			if (GUILayout.Button("START", GUILayout.Width(200f), GUILayout.Height(25f)))
			{
				SetState(GameState.Playing);
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}
#endif