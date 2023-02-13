#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity.EditorExtensions
{
	public static class EditorExtensions
	{
		private static bool saveOnPlay;
		public static bool SaveOnPlay
		{
			get
			{
				if (!valuesUpdated)
				{
					UpdateLocalValues();
				}
				
				return saveOnPlay;
			}
			set
			{
				saveOnPlay = value;
				EditorPrefs.SetBool(Application.productName + nameof(saveOnPlay), saveOnPlay);
			}
		}

		private static bool loadSceneOnPlay;
		public static bool LoadSceneOnPlay
		{
			get
			{
				if (!valuesUpdated)
				{
					UpdateLocalValues();
				}

				return loadSceneOnPlay;
			}
			set
			{
				loadSceneOnPlay = value;
				EditorPrefs.SetBool(Application.productName + nameof(loadSceneOnPlay), loadSceneOnPlay);
			}
		}

		private static string masterScene;
		public static string MasterScene 
		{
			get
			{
				if (!valuesUpdated)
				{
					UpdateLocalValues();
				}

				return masterScene; 
			}
			set
			{
				masterScene = value;
				EditorPrefs.SetString(Application.productName + nameof(masterScene), masterScene);
			}
		}
        
		public static string PreviousScene { get; set; }

		private static bool valuesUpdated;

		private static void UpdateLocalValues()
		{
			saveOnPlay = EditorPrefs.GetBool(Application.productName + nameof(saveOnPlay), true);
			loadSceneOnPlay = EditorPrefs.GetBool(Application.productName + nameof(loadSceneOnPlay), false);
			masterScene = EditorPrefs.GetString(Application.productName + nameof(masterScene), string.Empty);

			valuesUpdated = true;
		}
	}
}
#endif