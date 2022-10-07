#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;

namespace Toolnity.Shortcuts
{
	[Overlay(typeof(SceneView), ID, ID, true)]
	public class Toolbar : ToolbarOverlay
	{
		private const string ID = "Toolnity Shortcuts Toolbar";
        
		public Toolbar() : base (SaveAllToolbarButton.ID, TeleportToolbarButtons.ID, SettingsToolbarButton.ID)
		{
		}
	}
}
#endif