#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace Toolnity
{
	[Overlay(typeof(SceneView), ID, ID, true)]
	public class StaticCustomButtonsToolbar : ToolbarOverlay
	{
		private const string ID = "Toolnity Static CustomButtons";
		
		private StaticCustomButtonsToolbar() : base(StaticCustomButtons.ID) { }
	}
	
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class StaticCustomButtons : VisualElement
	{
		public const string ID = "Toolnity/StaticCustomButtons";
		private const string DEFAULT_ICON = "cs Script Icon";
		
		private static StaticCustomButtons instance;

		private StaticCustomButtons()
		{
			style.flexWrap = Wrap.Wrap;
			
			instance = this;
			AddStaticCustomButtonsInProject();
		}
		
		[UnityEditor.Callbacks.DidReloadScripts]
		private static void AddButtonsAfterScriptsCompile()
		{
			if(EditorApplication.isCompiling || EditorApplication.isUpdating)
			{
				EditorApplication.delayCall += AddButtonsAfterScriptsCompile;
				return;
			}

			if (instance == null)
			{
				instance = new StaticCustomButtons();
			}
			else
			{
				instance.AddStaticCustomButtonsInProject();
			}
		}

		private void AddStaticCustomButtonsInProject()
		{
			Clear();
			
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
						if (Attribute.GetCustomAttribute(methods[k], typeof(CustomButton)) is CustomButton attribute)
						{
							if (hierarchy.childCount > 0)
							{
								AddSpace();
							}
							var button = CreateButton(allTypes[j], methods[k], attribute);
							Add(button);
						}
					}
				}
			}
		}

		private static EditorToolbarButton CreateButton(Type currentClass, MethodBase method, CustomButton customButton)
		{
			CustomButtonsMenu.GetStaticButtonPathAndName(currentClass, method, customButton, out var path, out var name);

			var button = new EditorToolbarButton
			{
				name = name,
				text = name,
				tooltip = currentClass.Name + "::" + method.Name
			};
			
			string iconName;
			if (string.IsNullOrEmpty(customButton.Icon))
			{
				iconName = DEFAULT_ICON;
			}
			else
			{
				iconName = customButton.Icon;
			}

			var iconSet = false;
			try
			{
				var iconContent = EditorGUIUtility.IconContent(iconName);
				if (iconContent != null && iconContent.image != null)
				{
					iconSet = true;
					button.icon = iconContent.image as Texture2D;
				}
			}
			catch
			{
				// ignored
			}
			
			if (!iconSet)
			{
				button.icon = EditorGUIUtility.IconContent(DEFAULT_ICON).image as Texture2D;
			}
							
			button.clicked += () => method.Invoke(null, null);

			return button;
		}


		private void AddSpace()
		{
			var space = new Label
			{
				style =
				{
					height = 3f
				}
			};
			Add(space);
		}
	}
}
#endif