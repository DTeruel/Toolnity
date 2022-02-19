#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.SceneManagement.EditorSceneManager;

namespace Toolnity
{
	[InitializeOnLoad]
	public static class SceneSelector
	{
		public const string SCENE_SELECTOR_ENABLED = "Toolnity/Scene Selector/Enabled";
		private const string SCENE_SELECTOR_JUST_SCENES_IN_BUILD = "Toolnity/Scene Selector/JustScenesInBuild";

		private static bool justScenesInBuild;
		private static bool showSceneLauncher;
		private static readonly List<string> NamesList = new List<string>();
		private static readonly List<string> PathsList = new List<string>();
		private static string buttonText;

		private static GUIStyle popupMiddleAlignment;

		static SceneSelector()
		{
			UpdateScenes();
		}

		private static void ToggleJustScenesInBuild()
		{
			justScenesInBuild = !justScenesInBuild;
			EditorPrefs.SetBool(Application.dataPath + SCENE_SELECTOR_JUST_SCENES_IN_BUILD, justScenesInBuild);

			UpdateScenes();
		}

		public static void DrawGUI()
		{
			var optionEnabled = EditorPrefs.GetBool(Application.dataPath + SCENE_SELECTOR_ENABLED, true);
			if (!optionEnabled)
			{
				return;
			}
			
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			if (showSceneLauncher && 
			    ToolnitySettingsRegister.MenuPositionSelection != ToolnitySettingsRegister.MENU_POSITION_TOP &&
			    ToolnitySettingsRegister.MenuPositionSelection != ToolnitySettingsRegister.MENU_POSITION_BOTTOM)
			{
				GUILayout.Space(10);
			}
			if (GUILayout.Button(buttonText, GUILayout.Width(25)))
			{
				showSceneLauncher = !showSceneLauncher;
				UpdateScenes();
			}
			if (showSceneLauncher && 
			    ToolnitySettingsRegister.MenuPositionSelection != ToolnitySettingsRegister.MENU_POSITION_TOP &&
			    ToolnitySettingsRegister.MenuPositionSelection != ToolnitySettingsRegister.MENU_POSITION_BOTTOM)
			{
				GUILayout.Space(10);
			}
			GUILayout.EndVertical();
			
			if (showSceneLauncher)
			{
				buttonText = "X";

				GUILayout.BeginVertical();
				CheckStyles();
				var newSelection = EditorGUILayout.Popup(0, NamesList.ToArray(), popupMiddleAlignment);
				if (newSelection > 0)
				{
					AutoSave();
					OpenScene(PathsList[newSelection]);
				}
				
				GUILayout.BeginHorizontal();
				GUILayout.Space(40);
				
				justScenesInBuild = EditorPrefs.GetBool(Application.dataPath + SCENE_SELECTOR_JUST_SCENES_IN_BUILD, false);
				var newJustScenesInBuild = GUILayout.Toggle(justScenesInBuild, "  Just Scenes on Build");
				if (justScenesInBuild != newJustScenesInBuild)
				{
					ToggleJustScenesInBuild();
				}
				GUILayout.Space(40);
				GUILayout.EndHorizontal();
				
				GUILayout.EndVertical();
			}
			else
			{
				buttonText = "S";
			}
			GUILayout.EndHorizontal();
		}

		private static void CheckStyles()
		{
			if (popupMiddleAlignment == null)
			{
				popupMiddleAlignment = GUI.skin.GetStyle("Popup");
				popupMiddleAlignment.alignment = TextAnchor.MiddleCenter;
				popupMiddleAlignment.fontSize = 12;
			}
		}

		private static void AutoSave()
		{
			var scenesWithChanges = false;
			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				if (scene.isDirty)
				{
					scenesWithChanges = true;
				}
			}

			if (scenesWithChanges)
			{
				if (SaveCurrentModifiedScenesIfUserWantsTo())
				{
					Debug.Log("- - - - - - - - - - - - - - - - - - - - - - - SCENES SAVED - - - - - - - - - - - - - - - - - - - - - - -");
				}
			}
		}

		private static void UpdateScenes()
		{
			NamesList.Clear();
			PathsList.Clear();
			NamesList.Add("- Select Scene - ");
			PathsList.Add(" ");
			
			if (justScenesInBuild)
			{
				GetScenesFromBuild();
			}
			else
			{
				GetScenesFromProject();
			}
		}

		private static void GetScenesFromBuild()
		{
			var sceneInfo = EditorBuildSettings.scenes;
			for (var i = 0; i < sceneInfo.Length; i++)
			{
				var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneInfo[i].path);
				if (scene)
				{
					NamesList.Add(scene.name);
					PathsList.Add(sceneInfo[i].path);
				}
			}
		}

		private static void GetScenesFromProject()
		{
			var guids = AssetDatabase.FindAssets("t:Scene");
			for (var i = 0; i < guids.Length; i++)
			{
				var path = AssetDatabase.GUIDToAssetPath(guids[i]);
				var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
				if (scene)
				{
					NamesList.Add(scene.name);
					PathsList.Add(path);
				}
			}
		}

		internal class SceneDropdownPostprocessor : AssetPostprocessor
		{
			private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
				string[] movedAssets, string[] movedFromAssetPaths)
			{
				UpdateScenes();
			}
		}
	}
}
#endif