#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;

namespace Toolnity.EditorExtensions
{
    [Overlay(typeof(SceneView), ID, ID, true)]
    public class Toolbar : ToolbarOverlay
    {
        private const string ID = "Toolnity Editor Extensions Toolbar";
        
        public Toolbar() : base (
            OnPlayToolbarButton.ID,
            SceneSelectorToolbarButton.ID,
            SaveAllToolbarButton.ID,
            ScriptableObjectViewerToolbarButton.ID,
            SettingsToolbarButton.ID
            )
        {
        }
    }
}
#endif