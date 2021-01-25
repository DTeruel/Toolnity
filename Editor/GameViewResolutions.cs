#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;

namespace Toolnity
{
	public static class GameViewResolutions
	{
		#region Context Menu
		private struct ResolutionInfo
		{
			public GameViewSizeType SizeType;
			public int Width;
			public int Height;
			public string Name;
		}

		private static readonly ResolutionInfo[] PortraitRatios =
		{
			new ResolutionInfo
			{
				SizeType = GameViewSizeType.AspectRatio, Width = 18, Height = 39,
				Name = "[Port] iPhones Top - Ratio: 19.5:9"
			},
			new ResolutionInfo
				{SizeType = GameViewSizeType.AspectRatio, Width = 9, Height = 16, Name = "[Port] iPhone Old - Ratio"}
		};

		private static readonly ResolutionInfo[] PortraitResolutions =
		{
			new ResolutionInfo
			{
				SizeType = GameViewSizeType.FixedResolution, Width = 1242, Height = 2688,
				Name = "[Port] iPhone 11 Pro Max"
			},
			new ResolutionInfo
			{
				SizeType = GameViewSizeType.FixedResolution, Width = 1125, Height = 2436, Name = "[Port] iPhone 11 Pro"
			},
			new ResolutionInfo
				{SizeType = GameViewSizeType.FixedResolution, Width = 828, Height = 1792, Name = "[Port] iPhone 11"},
			new ResolutionInfo
			{
				SizeType = GameViewSizeType.FixedResolution, Width = 1080, Height = 1920, Name = "[Port] iPhone 7+/8+"
			},
			new ResolutionInfo
				{SizeType = GameViewSizeType.FixedResolution, Width = 750, Height = 1334, Name = "[Port] iPhone 8"},
			new ResolutionInfo
			{
				SizeType = GameViewSizeType.FixedResolution, Width = 640, Height = 1136, Name = "[Port] iPhone 5/5c/5S"
			}
		};

		private static readonly ResolutionInfo[] LandscapeRatios =
		{
			new ResolutionInfo
			{
				SizeType = GameViewSizeType.AspectRatio, Width = 39, Height = 18,
				Name = "[Land] iPhones Top - Ratio: 19.5:9"
			},
			new ResolutionInfo
				{SizeType = GameViewSizeType.AspectRatio, Width = 16, Height = 9, Name = "[Land] iPhones Meh - Ratio"}
		};

		private static readonly ResolutionInfo[] LandscapeResolutions =
		{
			new ResolutionInfo
			{
				SizeType = GameViewSizeType.FixedResolution, Width = 2688, Height = 1242,
				Name = "[Land] iPhone 11 Pro Max"
			},
			new ResolutionInfo
			{
				SizeType = GameViewSizeType.FixedResolution, Width = 2436, Height = 1125, Name = "[Land] iPhone 11 Pro"
			},
			new ResolutionInfo
				{SizeType = GameViewSizeType.FixedResolution, Width = 1792, Height = 828, Name = "[Land] iPhone 11"},
			new ResolutionInfo
			{
				SizeType = GameViewSizeType.FixedResolution, Width = 1920, Height = 1080, Name = "[Land] iPhone 7+/8+"
			},
			new ResolutionInfo
				{SizeType = GameViewSizeType.FixedResolution, Width = 1334, Height = 750, Name = "[Land] iPhone 8"},
			new ResolutionInfo
			{
				SizeType = GameViewSizeType.FixedResolution, Width = 1136, Height = 640, Name = "[Land] iPhone 5/5c/5S"
			}
		};


		[MenuItem("Tools/Toolnity/Game View/Add Portrait Aspect Ratios")]
		public static void AddPortraitAspectRatios()
		{
			foreach (var resolution in PortraitRatios)
			{
				AddResolution(resolution.SizeType, resolution.Width, resolution.Height, resolution.Name);
			}
		}

		[MenuItem("Tools/Toolnity/Game View/Add Portrait Resolutions")]
		public static void AddPortraitResolutions()
		{
			foreach (var resolution in PortraitResolutions)
			{
				AddResolution(resolution.SizeType, resolution.Width, resolution.Height, resolution.Name);
			}
		}

		[MenuItem("Tools/Toolnity/Game View/Add Landscape Aspect Ratios")]
		public static void AddLandscapeAspectRatios()
		{
			foreach (var resolution in LandscapeRatios)
			{
				AddResolution(resolution.SizeType, resolution.Width, resolution.Height, resolution.Name);
			}
		}

		[MenuItem("Tools/Toolnity/Game View/Add Landscape Resolutions")]
		public static void AddLandscapeResolutions()
		{
			foreach (var resolution in LandscapeResolutions)
			{
				AddResolution(resolution.SizeType, resolution.Width, resolution.Height, resolution.Name);
			}
		}

		[MenuItem("Tools/Toolnity/Game View/Clean Custom Resolutions")]
		public static void CleanCustomResolutions()
		{
			foreach (var resolution in PortraitRatios)
			{
				var id = FindSize(resolution.Name);
				if (id != -1)
				{
					DeleteCustomSizeId(id);
				}
			}

			foreach (var resolution in PortraitResolutions)
			{
				var id = FindSize(resolution.Name);
				if (id != -1)
				{
					DeleteCustomSizeId(id);
				}
			}

			foreach (var resolution in LandscapeRatios)
			{
				var id = FindSize(resolution.Name);
				if (id != -1)
				{
					DeleteCustomSizeId(id);
				}
			}

			foreach (var resolution in LandscapeResolutions)
			{
				var id = FindSize(resolution.Name);
				if (id != -1)
				{
					DeleteCustomSizeId(id);
				}
			}
		}

		private static void AddResolution(GameViewSizeType sizeType, int width, int height, string name)
		{
			var index = FindSize(name);
			if (index == -1)
			{
				AddCustomSize(sizeType, GameViewSizeGroupType.Standalone, width, height, name);
			}
		}
		#endregion

		#region Reflexion Utilities
		private enum GameViewSizeType
		{
			AspectRatio,
			FixedResolution
		}

		private static readonly object GameViewSizesInstance;
		private static readonly MethodInfo GetGroupMethod;

		static GameViewResolutions()
		{
			var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
			var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
			var instanceProp = singleType.GetProperty("instance");
			GetGroupMethod = sizesType.GetMethod("GetGroup");
			if (instanceProp != null) GameViewSizesInstance = instanceProp.GetValue(null, null);
		}

		private static void SetSize(int index)
		{
			var gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
			var gameViewWindow = EditorWindow.GetWindow(gameViewType);
			var sizeSelectionCallback = gameViewType.GetMethod("SizeSelectionCallback");
			if (sizeSelectionCallback != null) sizeSelectionCallback.Invoke(gameViewWindow, new object[] {index, null});
		}

		private static void DeleteCustomSizeId(int id,
			GameViewSizeGroupType sizeGroupType = GameViewSizeGroupType.Standalone)
		{
			var group = GetGroup(sizeGroupType);
			var removeCustomSize = GetGroupMethod.ReturnType.GetMethod("RemoveCustomSize");
			if (removeCustomSize != null) removeCustomSize.Invoke(group, new object[] {id});
		}

		private static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width,
			int height, string text)
		{
			var type = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");

			var group = GetGroup(sizeGroupType);
			var addCustomSize = GetGroupMethod.ReturnType.GetMethod("AddCustomSize");
			var gameViewSizeType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");

			var constructor = gameViewSizeType.GetConstructor(new[] {type, typeof(int), typeof(int), typeof(string)});
			if (constructor == null) return;
			var newSize = constructor.Invoke(new object[] {(int) viewSizeType, width, height, text});

			if (addCustomSize != null) addCustomSize.Invoke(group, new[] {newSize});
		}

		private static int FindSize(string text, GameViewSizeGroupType sizeGroupType = GameViewSizeGroupType.Standalone)
		{
			var group = GetGroup(sizeGroupType);
			var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
			if (getDisplayTexts == null) return -1;
			if (!(getDisplayTexts.Invoke(group, null) is string[] displayTexts)) return -1;
			for (var i = 0; i < displayTexts.Length; i++)
			{
				var display = displayTexts[i];
				var pren = display.IndexOf('(');
				if (pren != -1)
					display = display.Substring(0, pren - 1);
				if (display == text)
					return i;
			}

			return -1;
		}

		private static int FindSize(int width, int height,
			GameViewSizeGroupType sizeGroupType = GameViewSizeGroupType.Standalone)
		{
			var group = GetGroup(sizeGroupType);
			var groupType = group.GetType();

			var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
			var getCustomCount = groupType.GetMethod("GetCustomCount");
			if (getBuiltinCount == null) return -1;
			if (getCustomCount == null) return -1;
			var sizesCount = (int) getBuiltinCount.Invoke(group, null) + (int) getCustomCount.Invoke(group, null);

			var getGameViewSize = groupType.GetMethod("GetGameViewSize");
			if (getGameViewSize == null) return -1;
			var gvsType = getGameViewSize.ReturnType;

			var widthProp = gvsType.GetProperty("width");
			var heightProp = gvsType.GetProperty("height");

			var indexValue = new object[1];
			for (var i = 0; i < sizesCount; i++)
			{
				indexValue[0] = i;
				var size = getGameViewSize.Invoke(group, indexValue);
				if (widthProp == null) continue;
				var sizeWidth = (int) widthProp.GetValue(size, null);
				if (heightProp == null) continue;
				var sizeHeight = (int) heightProp.GetValue(size, null);
				if (sizeWidth == width && sizeHeight == height)
					return i;
			}

			return -1;
		}

		private static object GetGroup(GameViewSizeGroupType type)
		{
			return GetGroupMethod.Invoke(GameViewSizesInstance, new object[] {(int) type});
		}
		#endregion
	}
}
#endif