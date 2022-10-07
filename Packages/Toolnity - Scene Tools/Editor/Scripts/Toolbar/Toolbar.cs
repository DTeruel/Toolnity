#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;

namespace Toolnity.SceneTools
{
	[Overlay(typeof(SceneView), ID, ID, true)]
	public class Toolbar : ToolbarOverlay
	{
		private const string ID = "Toolnity Scene Tools Toolbar";
        
		public Toolbar() : base (PropPlacementToolbarButton.ID, ReplaceToolToolbarButton.ID)
		{
		}
	}
}
#endif