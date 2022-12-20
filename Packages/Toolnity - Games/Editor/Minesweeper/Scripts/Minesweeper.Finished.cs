#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Toolnity.Games
{
	public partial class Minesweeper
	{
		private static void DrawFinishedState()
		{
			DrawTopBarFinished();
			EditorGUI.BeginDisabledGroup(true);
			DrawGrid();
			EditorGUI.EndDisabledGroup();
		}

		private static void DrawTopBarFinished()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Time: " + TimeSpan.FromSeconds(totalTime).ToString("mm\\:ss"));
			if (GUILayout.Button("Next", GUILayout.Width(75)))
			{
				SetState(GameState.Ranking);
			}
			GUILayout.EndHorizontal();
		}
	}
}
#endif