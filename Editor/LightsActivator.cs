#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
	public static class LightsActivator
	{
		public const string LIGHTS_ACTIVATOR_ENABLED = "Toolnity/LightsActivator/Enabled";

		private static bool showLightsActivator;
		private static GUIStyle popupMiddleAlignment;
		private static string buttonText;

		public static void DrawGUI()
		{
			var enabledOption = EditorPrefs.GetBool(Application.dataPath + LIGHTS_ACTIVATOR_ENABLED, true);
			if (!enabledOption)
			{
				return;
			}

			if (popupMiddleAlignment == null)
			{
				popupMiddleAlignment = GUI.skin.GetStyle("Popup");
				popupMiddleAlignment.alignment = TextAnchor.MiddleCenter;
				popupMiddleAlignment.fontSize = 12;
			}

			
			if (GUILayout.Button(buttonText))
			{
				showLightsActivator = !showLightsActivator;
			}
			
			if (showLightsActivator)
			{
				buttonText = "<";

			}
			else
			{
				buttonText = "L";
			}
		}
	}
}
#endif