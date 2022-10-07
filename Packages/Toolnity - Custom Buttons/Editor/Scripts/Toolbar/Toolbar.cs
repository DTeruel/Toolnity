#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;

namespace Toolnity.CustomButtons
{
	[Overlay(typeof(SceneView), ID, ID, true)]
	public class Toolbar : ToolbarOverlay
	{
		private const string ID = "Toolnity Custom Buttons Toolbar";
		
		private Toolbar() : base(
			CustomButtonsToolbarButton.ID,
			StaticCustomButtonsToolbarButton.ID,
			SettingsToolbarButton.ID
			)
		{ }
	}
}
#endif