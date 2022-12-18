#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;

namespace Toolnity.ProjectInfo
{
    [Overlay(typeof(SceneView), ID, ID, true)]
    public class Toolbar : ToolbarOverlay
    {
        private const string ID = "Toolnity Project Info Toolbar";
        
        public Toolbar() : base (
            ProjectLinksToolbarButton.ID,
            SettingsToolbarButton.ID
            )
        {
        }
    }
}
#endif