#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// Original idea from: https://gist.github.com/Jquirky/

namespace Toolnity.HierarchyWindowExtensions
{
	[InitializeOnLoad]
	public static class HierarchyColorizer
	{
		private const string UNDO_APPLY_COLOR_NAME = "Modify Hierarchy Color";
		private static readonly Vector2 ColorPickerWindowSize = new (200, 25);

		static HierarchyColorizer()
		{
			EditorApplication.hierarchyWindowItemOnGUI += hierarchyWindowItemOnGUI;
		}

		private static readonly GUIStyle EnabledGuiStyle = new GUIStyle
		{
			normal = {textColor = Color.white},
			alignment = TextAnchor.MiddleCenter,
			fontStyle = FontStyle.Bold
		};

		private static readonly GUIStyle DisabledGuiStyle = new GUIStyle
		{
			normal = {textColor = Color.gray},
			alignment = TextAnchor.MiddleCenter,
			fontStyle = FontStyle.Italic
		};

		private static void hierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
		{
			var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			if (gameObject == null)
			{
				return;
			}

			var objectName = gameObject.name;
			if (!objectName.StartsWith("[#", StringComparison.Ordinal))
			{
				return;
			}

			objectName = objectName.ToUpperInvariant();

			var colorString = objectName.Substring(1, 7);
			objectName = objectName.Remove(0, 9);
			
			if (ColorUtility.TryParseHtmlString(colorString, out var rectColor))
			{
				EditorGUI.DrawRect(selectionRect, rectColor);
				if (gameObject.activeSelf)
				{
					EditorGUI.LabelField(selectionRect, objectName, EnabledGuiStyle);
				}
				else
				{
					EditorGUI.LabelField(selectionRect, objectName, DisabledGuiStyle);
				}
			}
		}
		
		[MenuItem("GameObject/Toolnity/Colorizer/Pick Color", false, PRIORITY)]
		public static void ColorizePickColor()
		{
			var colorPickerWindow = (ColorPickerWindow)EditorWindow.GetWindow(typeof(ColorPickerWindow), true, "Color Picker");
			colorPickerWindow.minSize = ColorPickerWindowSize;
			colorPickerWindow.maxSize = ColorPickerWindowSize;
			var startColor = GetColorFromName();
			
			colorPickerWindow.Init(startColor, (color) =>
			{
				var stringColor = ColorUtility.ToHtmlStringRGB(color);
				AddColorToName(stringColor);
			});
			colorPickerWindow.ShowModalUtility();
		}

		private static Color GetColorFromName()
		{
			if (Selection.objects.Length > 0)
			{
				var currentObject = Selection.objects[0];
				if(currentObject.name.StartsWith("[#", StringComparison.Ordinal))
				{
					string htmlColorString = currentObject.name.Substring(1, 7);
					ColorUtility.TryParseHtmlString(htmlColorString, out var color);
					return color;
				}
			}
			
			return Color.green;
		}

		private const int PRIORITY = 1;
		
		[MenuItem("GameObject/Toolnity/Colorizer/Default Colors/Black", false, PRIORITY)]
		public static void ColorizeInBlack()
		{
			AddColorToName("000000");
		}

		[MenuItem("GameObject/Toolnity/Colorizer/Default Colors/Gray", false, PRIORITY)]
		public static void ColorizeInGray()
		{
			AddColorToName("383838");
		}

		[MenuItem("GameObject/Toolnity/Colorizer/Default Colors/Purple", false, PRIORITY)]
		public static void ColorizeInPurple()
		{
			AddColorToName("493B73");
		}

		[MenuItem("GameObject/Toolnity/Colorizer/Default Colors/Green", false, PRIORITY)]
		public static void ColorizeInGreen()
		{
			AddColorToName("4A6A11");
		}

		[MenuItem("GameObject/Toolnity/Colorizer/Default Colors/Emerald", false, PRIORITY)]
		public static void ColorizeInEmerald()
		{
			AddColorToName("2D6464");
		}

		[MenuItem("GameObject/Toolnity/Colorizer/Default Colors/Blue", false, PRIORITY)]
		public static void ColorizeInBlue()
		{
			AddColorToName("394970");
		}

		[MenuItem("GameObject/Toolnity/Colorizer/Default Colors/Red", false, PRIORITY)]
		public static void ColorizeInRed()
		{
			AddColorToName("A74C4C");
		}

		[MenuItem("GameObject/Toolnity/Colorizer/Default Colors/Pink", false, PRIORITY)]
		public static void ColorizeInPink()
		{
			AddColorToName("883E65");
		}

		[MenuItem("GameObject/Toolnity/Colorizer/Default Colors/Orange", false, PRIORITY)]
		public static void ColorizeInOrange()
		{
			AddColorToName("733012");
		}

		[MenuItem("GameObject/Toolnity/Colorizer/Default Colors/Yellow", false, PRIORITY)]
		public static void ColorizeInYellow()
		{
			AddColorToName("735F12");
		}

		private static void AddColorToName(string color)
		{
			Undo.RecordObjects(Selection.objects, UNDO_APPLY_COLOR_NAME);
			
			foreach (var currentObject in Selection.objects)
			{
				var objectName = currentObject.name;
				
				if (objectName.StartsWith("[#"))
				{
					objectName = objectName.Remove(1, 7);
					objectName = objectName.Insert(1, $"#{color}");
				}
				else
				{
					objectName = objectName.Insert(0, $"[#{color}]");
				}

				currentObject.name = objectName;
			}
		}

		[MenuItem("GameObject/Toolnity/Colorizer/Remove Color", false, PRIORITY)]
		public static void RemoveColorFromName()
		{
			Undo.RecordObjects(Selection.objects, UNDO_APPLY_COLOR_NAME);
			
			foreach (var currentObject in Selection.objects)
			{
				if (currentObject.name.StartsWith("[#", StringComparison.Ordinal))
				{
					currentObject.name = currentObject.name.Remove(0, 9);
				}
			}
		}
	}
}
#endif