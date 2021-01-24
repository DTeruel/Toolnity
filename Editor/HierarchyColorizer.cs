#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// Original idea from: https://gist.github.com/Jquirky/

namespace Toolnity
{
	[InitializeOnLoad]
	public class HierarchyColorizer
	{
		private const string UNDO_APPLY_COLOR_NAME = "Modify Hierarchy Color";

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

		[MenuItem("GameObject/Colorizer/Colorize/Black", false, -100)]
		public static void ColorizeInBlack()
		{
			AddColorToName("000000");
		}

		[MenuItem("GameObject/Colorizer/Colorize/Gray", false, -100)]
		public static void ColorizeInGray()
		{
			AddColorToName("383838");
		}

		[MenuItem("GameObject/Colorizer/Colorize/Purple", false, -100)]
		public static void ColorizeInPurple()
		{
			AddColorToName("493B73");
		}

		[MenuItem("GameObject/Colorizer/Colorize/Green", false, -100)]
		public static void ColorizeInGreen()
		{
			AddColorToName("4A6A11");
		}

		[MenuItem("GameObject/Colorizer/Colorize/Emerald", false, -100)]
		public static void ColorizeInEmerald()
		{
			AddColorToName("2D6464");
		}

		[MenuItem("GameObject/Colorizer/Colorize/Blue", false, -100)]
		public static void ColorizeInBlue()
		{
			AddColorToName("394970");
		}

		[MenuItem("GameObject/Colorizer/Colorize/Red", false, -100)]
		public static void ColorizeInRed()
		{
			AddColorToName("A74C4C");
		}

		[MenuItem("GameObject/Colorizer/Colorize/Pink", false, -100)]
		public static void ColorizeInPink()
		{
			AddColorToName("883E65");
		}

		[MenuItem("GameObject/Colorizer/Colorize/Orange", false, -100)]
		public static void ColorizeInOrange()
		{
			AddColorToName("733012");
		}

		[MenuItem("GameObject/Colorizer/Colorize/Yellow", false, -100)]
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

		[MenuItem("GameObject/Colorizer/Remove Color", false, -100)]
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