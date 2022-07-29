#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class CustomButtonsToolbar : EditorToolbarDropdown
	{
		public const string ID = "Toolnity/CustomButtonsToolbar";
		
		private static string dropChoice;
		private static GenericMenu menu;

		public CustomButtonsToolbar()
		{
			text = "Custom";
			tooltip = "Custom Buttons";
			icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
			
			clicked += ShowDropdown;
		}

		private static void GenerateCustomButtons()
		{
			menu = new GenericMenu();
			AddNonStaticCustomButtonsInScene(menu);
			menu.AddSeparator(string.Empty);
			AddStaticCustomButtonsInProject(menu);
		}

		private static void ShowDropdown()
		{
			GenerateCustomButtons();
			menu.ShowAsContext();
		}
	
		private static void AddNonStaticCustomButtonsInScene(GenericMenu genericMenu)
		{
			var sceneActive = Object.FindObjectsOfType<MonoBehaviour>();
			foreach (var mono in sceneActive)
			{
				var monoType = mono.GetType();
				var methods = monoType.GetMethods(
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				for (var i = 0; i < methods.Length; i++) 
				{
					if (Attribute.GetCustomAttribute(methods[i], typeof(CustomButton)) is CustomButton customButton)
					{
						var method = methods[i];
						var buttonName = CustomButtonsMenu.GetNonStaticButtonName(monoType, method, customButton, mono);
						genericMenu.AddItem(
							new GUIContent(buttonName), 
							false,
							() =>
							{
								method.Invoke(mono, null);
							});
					}
				}
			}
		}

		private static void AddStaticCustomButtonsInProject(GenericMenu genericMenu)
		{
			var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (var i = 0; i < allAssemblies.Length; i++)
			{
				var allTypes = allAssemblies[i].GetTypes();
				for (var j = 0; j < allTypes.Length; j++)
				{
					var methods = allTypes[j].GetMethods(
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

					for (var k = 0; k < methods.Length; k++)
					{
						if (Attribute.GetCustomAttribute(methods[k], typeof(CustomButton)) is CustomButton customButton)
						{
							var method = methods[k];
							var buttonName = CustomButtonsMenu.GetStaticButtonName(allTypes[j], method, customButton);
							genericMenu.AddItem(
								new GUIContent(buttonName), 
								false,
								() =>
								{
									method.Invoke(null, null);
								});
						}
					}
				}
			}
		}
	}
}
#endif