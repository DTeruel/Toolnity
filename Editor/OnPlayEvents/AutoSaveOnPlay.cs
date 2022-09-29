#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using static UnityEditor.SceneManagement.EditorSceneManager;

namespace Toolnity
{
	[InitializeOnLoad]
	public class AutoSaveOnPlay : EditorWindow
	{
		public const string AUTO_SAVE_SETTINGS_ENABLED = "Toolnity/Auto Save On Play/Enabled";
		
		public static bool Enabled
		{
			get => EditorPrefs.GetBool(Application.dataPath + AUTO_SAVE_SETTINGS_ENABLED, true);
			private set => EditorPrefs.SetBool(Application.dataPath + AUTO_SAVE_SETTINGS_ENABLED, value);
		}
		
		static AutoSaveOnPlay()
		{
			EditorApplication.playModeStateChanged += SaveProject;
		}

		public static void ToggleEnabled()
		{
			Enabled = !Enabled;
		}
		
		private static void SaveProject(PlayModeStateChange state)
		{
			if (!Enabled)
			{
				return;
			}

			if (EditorApplication.isPlaying)
			{
				return;
			}

			BasicShortcuts.SaveScenes();
			BasicShortcuts.SavePrefab();
			BasicShortcuts.SaveProject();
		}
	}
}
#endif
