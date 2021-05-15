#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using static UnityEditor.SceneManagement.EditorSceneManager;

namespace Toolnity
{
	[InitializeOnLoad]
	public class AutoSave : EditorWindow
	{
		public const string AUTO_SAVE_SETTINGS_ENABLED = "Toolnity/Auto Save/Enabled";
		
		static AutoSave()
		{
			EditorApplication.playModeStateChanged += AutoSaveOnPlay;
		}
		
		private static void AutoSaveOnPlay(PlayModeStateChange state)
		{
			var enabledOption = EditorPrefs.GetBool(Application.dataPath + AUTO_SAVE_SETTINGS_ENABLED, true);
			if (!enabledOption)
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
				if (SaveCurrentModifiedScenesIfUserWantsTo())
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
