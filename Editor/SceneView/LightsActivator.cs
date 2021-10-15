#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
	[InitializeOnLoad]
	public static class LightsActivator
	{
		public const string LIGHTS_ACTIVATOR_ENABLED = "Toolnity/LightsActivator/Enabled";

		private static readonly List<string> OptionsSelectionList = new List<string> {
				"- Select Action -", 
				"Activate All Lights", 
				"Deactivate All Lights", 
				"Activate Not Baked Lights", 
				"Deactivate Not Baked Lights", 
				"Activate Baked Lights", 
				"Deactivate Baked Lights", 
				"Bake Lights",
				"Turn On All Lights + Bake Light + Turn Off Baked Lights",
				"Turn On Baked Lights + Bake Light + Turn Off Baked Lights",
				"Cancel Bake",
				"Clear Bake Data"
		};
		private const int ActivateAllLights = 1;
		//private const int DeactivateAllLights = 2;
		private const int ActivateNotBakedLights = 3;
		private const int DeactivateNotBakedLights = 4;
		private const int ActivateBakedLights = 5;
		private const int DeactivateBakedLights = 6;
		private const int BakeScene = 7;
		private const int TurnOnAllLightsBakeSceneAndTurnOffBakedLights = 8;
		private const int TurnOnBakedLightsBakeSceneAndTurnOffBakedLights = 9;
		private const int CancelBake = 10;
		private const int ClearBakeData = 11;

		private static bool showLightsActivator;
		private static GUIStyle popupMiddleAlignment;
		private static string buttonText;
		private static bool turnOffBakedLightsAfterBake;

		static LightsActivator()
		{
			Lightmapping.bakeCompleted += LightmappingOnBakeCompleted;
		}

		private static void LightmappingOnBakeCompleted()
		{
			if (turnOffBakedLightsAfterBake)
			{
				turnOffBakedLightsAfterBake = false;
				ActivateLights(false, true);
			}
		}

		public static void DrawGUI()
		{
			var enabledOption = EditorPrefs.GetBool(Application.dataPath + LIGHTS_ACTIVATOR_ENABLED, true);
			if (!enabledOption)
			{
				return;
			}
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(buttonText, GUILayout.Width(25)))
			{
				showLightsActivator = !showLightsActivator;
			}
			
			if (showLightsActivator)
			{
				buttonText = "X";
				
				CheckStyles();
				var newSelection = EditorGUILayout.Popup(0, OptionsSelectionList.ToArray(), popupMiddleAlignment);
				if (newSelection > 0)
				{
					ApplyAction(newSelection);
				}
			}
			else
			{
				buttonText = "L";
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

		private static void ApplyAction(int option)
		{
			turnOffBakedLightsAfterBake = false;
			
			if (option == BakeScene)
			{
				Lightmapping.BakeAsync();
			}
			else if (option == TurnOnAllLightsBakeSceneAndTurnOffBakedLights)
			{
				turnOffBakedLightsAfterBake = true;
				ActivateLights(true);
				Lightmapping.BakeAsync();
			}
			else if (option == TurnOnBakedLightsBakeSceneAndTurnOffBakedLights)
			{
				turnOffBakedLightsAfterBake = true;
				ActivateLights(true, true);
				Lightmapping.BakeAsync();
			}
			else if (option == ClearBakeData)
			{
				Lightmapping.Clear();
				Lightmapping.ClearDiskCache();
				Lightmapping.ClearLightingDataAsset();
			}
			else if (option == CancelBake)
			{
				Lightmapping.Cancel();
			}
			else
			{
				var enableComponent = option == ActivateAllLights || option == ActivateBakedLights || option == ActivateNotBakedLights;
				var justBaked = option == ActivateBakedLights || option == DeactivateBakedLights;
				var justNotBaked = option == ActivateNotBakedLights || option == DeactivateNotBakedLights;
				ActivateLights(enableComponent, justBaked, justNotBaked);
			}
		}

		private static void ActivateLights(bool enableComponent, bool justBaked = false, bool justNotBaked = false)
		{
			var allLightComponents = Object.FindObjectsOfType<Light>();
			for (var i = 0; i < allLightComponents.Length; i++)
			{
				var bakedLight = allLightComponents[i].lightmapBakeType == LightmapBakeType.Baked || allLightComponents[i].type == LightType.Area;
				if ((justBaked && !bakedLight) || (justNotBaked && bakedLight))
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