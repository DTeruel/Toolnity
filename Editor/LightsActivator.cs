#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
	public static class LightsActivator
	{
		public const string LIGHTS_ACTIVATOR_ENABLED = "Toolnity/LightsActivator/Enabled";

		private static readonly List<string> OptionsSelectionList = new List<string> {
				"- Select Action -", 
				"Activate All Lights", 
				"Deactivate All Lights", 
				"Activate Baked Lights", 
				"Deactivate Baked Lights"
		};
		private const int ActivateAllLights = 1;
		private const int DeactivateAllLights = 2;
		private const int ActivateBakedLights = 3;
		private const int DeactivateBakedLights = 4;

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
			
			if (GUILayout.Button(buttonText))
			{
				showLightsActivator = !showLightsActivator;
			}
			
			if (showLightsActivator)
			{
				buttonText = "<";
				
				CheckStyles();
				var newSelection = EditorGUILayout.Popup(0, OptionsSelectionList.ToArray(), popupMiddleAlignment);
				if (newSelection > 0)
				{
					ApplyAction(newSelection);
					showLightsActivator = false;
				}
			}
			else
			{
				buttonText = "L";
			}
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

		private static void ApplyAction(int option)
		{
			var enableComponent = option == ActivateAllLights || option == ActivateBakedLights;
			var justBaked = option == ActivateBakedLights || option == DeactivateBakedLights;
			ActivateLights(enableComponent, justBaked);
		}

		private static void ActivateLights(bool enableComponent, bool justBaked)
		{
			var allLightComponents = Object.FindObjectsOfType<Light>();
			for (var i = 0; i < allLightComponents.Length; i++)
			{
				if (justBaked && allLightComponents[i].lightmapBakeType != LightmapBakeType.Baked && allLightComponents[i].type != LightType.Area)
				{
					continue;
				}

				if (allLightComponents[i].enabled != enableComponent)
				{
					Undo.RegisterFullObjectHierarchyUndo(allLightComponents[i], "Change Component State");
					allLightComponents[i].enabled = enableComponent;
					if (enableComponent)
					{
						Debug.Log("[Toolnity] Light component enabled: " + allLightComponents[i].gameObject.name, allLightComponents[i].gameObject);
					}
					else
					{
						Debug.Log("[Toolnity] Light component disabled: " + allLightComponents[i].gameObject.name, allLightComponents[i].gameObject);
					}
				}
			}
		}
	}
}
#endif