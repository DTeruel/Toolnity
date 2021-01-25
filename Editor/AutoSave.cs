#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using static UnityEditor.SceneManagement.EditorSceneManager;

namespace Toolnity
{
	[InitializeOnLoad]
	public class AutoSave
	{
		private const string MENU_NAME = "Tools/Toolnity/Auto Save/Autosave On Run";
		private static bool isActive;

		static AutoSave()
		{
			EditorApplication.delayCall += () =>
			{
				isActive = EditorPrefs.GetBool(MENU_NAME, true);
				if (Menu.GetChecked(MENU_NAME))
				{
					Menu.SetChecked(MENU_NAME, isActive);
				}
				SetMode();
			};
		}

		[MenuItem(MENU_NAME)]
		private static void ToggleMode()
		{
			isActive = !isActive;
			Menu.SetChecked(MENU_NAME, isActive);
			EditorPrefs.SetBool(MENU_NAME, isActive);
			SetMode();
			
			Debug.Log("[Toolnity] AutoSave On Run: " + isActive);
		}

		private static void SetMode()
		{
			if (isActive)
			{
				EditorApplication.playModeStateChanged += AutoSaveOnRun;
			}
			else
			{
				EditorApplication.playModeStateChanged -= AutoSaveOnRun;
			}
		}

		private static void AutoSaveOnRun(PlayModeStateChange state)
		{
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
				SaveCurrentModifiedScenesIfUserWantsTo();
				Debug.Log("[Toolnity] - - - - - - - - - - - - - - - - - - - - - - - SCENES SAVED - - - - - - - - - - - - - - - - - - - - - - -");
			}

			AssetDatabase.SaveAssets();
			Debug.Log("[Toolnity] +++++++++++++++++++++++++ PROJECT SAVED +++++++++++++++++++++++++");
		}
	}
}
#endif
