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

		private const int ACTIVATE_ALL_LIGHTS = 0;
		//private const int DEACTIVATE_ALL_LIGHTS = 1;
		private const int ACTIVATE_NOT_BAKED_LIGHTS = 2;
		private const int DEACTIVATE_NOT_BAKED_LIGHTS = 3;
		private const int ACTIVATE_BAKED_LIGHTS = 4;
		private const int DEACTIVATE_BAKED_LIGHTS = 5;
		private const int BAKE_SCENE = 6;
		private const int TURN_ON_ALL_LIGHTS_BAKE_SCENE_AND_TURN_OFF_BAKED_LIGHTS = 7;
		private const int TURN_ON_BAKED_LIGHTS_BAKE_SCENE_AND_TURN_OFF_BAKED_LIGHTS = 8;
		private const int CANCEL_BAKE = 9;
		private const int CLEAR_BAKE_DATA = 10;
		
		public static readonly List<string> OptionsSelectionList = new List<string> {
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

		public static void ApplyOption(string selection)
		{
			for (var i = 0; i < OptionsSelectionList.Count; i++)
			{
				var option = OptionsSelectionList[i];
				if (selection == option)
				{
					ApplyAction(i);
				}
			}
		}
		
		private static void ApplyAction(int option)
		{
			turnOffBakedLightsAfterBake = false;
			
			if (option == BAKE_SCENE)
			{
				Lightmapping.BakeAsync();
			}
			else if (option == TURN_ON_ALL_LIGHTS_BAKE_SCENE_AND_TURN_OFF_BAKED_LIGHTS)
			{
				turnOffBakedLightsAfterBake = true;
				ActivateLights(true);
				Lightmapping.BakeAsync();
			}
			else if (option == TURN_ON_BAKED_LIGHTS_BAKE_SCENE_AND_TURN_OFF_BAKED_LIGHTS)
			{
				turnOffBakedLightsAfterBake = true;
				ActivateLights(true, true);
				Lightmapping.BakeAsync();
			}
			else if (option == CLEAR_BAKE_DATA)
			{
				Lightmapping.Clear();
				Lightmapping.ClearDiskCache();
				Lightmapping.ClearLightingDataAsset();
			}
			else if (option == CANCEL_BAKE)
			{
				Lightmapping.Cancel();
			}
			else
			{
				var enableComponent = option == ACTIVATE_ALL_LIGHTS || option == ACTIVATE_BAKED_LIGHTS || option == ACTIVATE_NOT_BAKED_LIGHTS;
				var justBaked = option == ACTIVATE_BAKED_LIGHTS || option == DEACTIVATE_BAKED_LIGHTS;
				var justNotBaked = option == ACTIVATE_NOT_BAKED_LIGHTS || option == DEACTIVATE_NOT_BAKED_LIGHTS;
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