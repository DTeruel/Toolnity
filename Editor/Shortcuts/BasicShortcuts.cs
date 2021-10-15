#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolnity
{
	public static class BasicShortcuts
	{
		[MenuItem("Tools/Toolnity/Shortcuts/Game Play | Stop _F5", priority = 10)]
		private static void PlayStop()
		{
			if (!ToolnitySettingsRegister.BasicShortcutsEnabled)
			{
				return;
			}
			
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
				return;
			}

			EditorApplication.EnterPlaymode();
		}

		[MenuItem("Tools/Toolnity/Shortcuts/Game Pause | Resume _F6", priority = 10)]
		private static void PauseResume()
		{
			if (!ToolnitySettingsRegister.BasicShortcutsEnabled)
			{
				return;
			}

			EditorApplication.isPaused = !EditorApplication.isPaused;
		}

		[MenuItem("Tools/Toolnity/Shortcuts/Game Next Frame _F7", priority = 10)]
		private static void NextFrame()
		{
			if (!ToolnitySettingsRegister.BasicShortcutsEnabled)
			{
				return;
			}

			EditorApplication.Step();
		}

		[MenuItem("Tools/Toolnity/Shortcuts/Project Save All _F12", priority = 100)]
		private static void SaveAll()
		{
			if (!ToolnitySettingsRegister.BasicShortcutsEnabled)
			{
				return;
			}

			if (EditorApplication.isPlaying)
			{
				return;
			}

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
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					Debug.Log("-------------------------------- SCENES SAVED --------------------------------");
				}
			}

			AssetDatabase.SaveAssets();
			Debug.Log("+++++++++++++++++++++++++ PROJECT SAVED +++++++++++++++++++++++++");
		}
	}
}
#endif