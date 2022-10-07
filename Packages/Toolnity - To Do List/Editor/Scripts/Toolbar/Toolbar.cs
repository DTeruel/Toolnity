#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;

namespace Toolnity.ToDoList
{
	[Overlay(typeof(SceneView), ID, ID, true)]
	public class Toolbar : ToolbarOverlay
	{
		private const string ID = "Toolnity To Do List Toolbar";
        
		public Toolbar() : base (ToDoListToolbarButton.ID)
		{
		}
	}
}
#endif