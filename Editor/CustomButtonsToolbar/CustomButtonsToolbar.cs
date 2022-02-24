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

		public CustomButtonsToolbar()
		{
			text = "Custom";
			tooltip = "Custom Buttons";
			icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
			
			clicked += ShowDropdown;
		}

		private static void ShowDropdown()
		{
			var menu = new GenericMenu();
			AddNonStaticCustomButtonsInScene(menu);
			menu.AddSeparator(string.Empty);
			AddStaticCustomButtonsInProject(menu);
			menu.ShowAsContext();
		}
	
		private static void AddNonStaticCustomButtonsInScene(GenericMenu menu)
		{
			var sceneActive = Object.FindObjectsOfType<MonoBehaviour>();
			foreach (var mono in sceneActive) 
			{
				var methods = mono.GetType().GetMethods(
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				for (var i = 0; i < methods.Length; i++) 
				{
					var attribute = Attribute.GetCustomAttribute(methods[i], typeof(CustomButton)) as CustomButton;
					if (attribute != null)
					{
						var method = methods[i];
						
						menu.AddItem(
							new GUIContent("[" + mono.name + "] " + mono.GetType().FullName + "::" + method.Name), 
							false,
							() =>
							{
								method.Invoke(mono, null);
							});
					}
				}
			}
		}

		private static void AddStaticCustomButtonsInProject(GenericMenu menu)
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
						var attribute = Attribute.GetCustomAttribute(methods[k], typeof(CustomButton)) as CustomButton;
						if (attribute != null)
						{
							var method = methods[k];
						
							menu.AddItem(
								new GUIContent(allTypes[j].Name + "::" + method.Name), 
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