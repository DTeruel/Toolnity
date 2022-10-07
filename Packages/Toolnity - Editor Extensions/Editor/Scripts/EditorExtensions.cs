using System.IO;
using UnityEditor;
using UnityEngine;

namespace Toolnity.EditorExtensions
{
	public static class EditorExtensions
	{
		private static EditorExtensionsConfig config;
		public static EditorExtensionsConfig Config
		{
			get
			{
				if (config == null)
				{
					LoadOrCreateConfig();
				}

				return config;
			}
		}

		private static void LoadOrCreateConfig()
		{
			var allAssets = Resources.LoadAll<EditorExtensionsConfig>("");
			if (allAssets.Length > 0)
			{
				config = allAssets[0];
				return;
			}

            #if UNITY_EDITOR
			Debug.Log("[Editor Extensions] No 'Editor Extensions Config' file found in the Resources folders. Creating a new one in \"\\Assets\\Resources\"");

			config = ScriptableObject.CreateInstance<EditorExtensionsConfig>();
			const string pathFolder = "Assets/Resources/";
			const string assetName = "Editor Extensions Config.asset";
			if (!Directory.Exists("Assets/Resources"))
			{
				Directory.CreateDirectory("Assets/Resources");
			}
			AssetDatabase.CreateAsset(config, pathFolder + assetName);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
            #else
            Debug.LogError("[Editor Extensions] No 'Editor Extensions Config' file found in the Resources folders. Create one in the editor. ");
            #endif
		}
	}
}