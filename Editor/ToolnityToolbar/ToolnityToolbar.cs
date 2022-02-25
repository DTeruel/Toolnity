#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;

namespace Toolnity
{
    [Overlay(typeof(SceneView), ID, ID, true)]
    public class ToolnityToolbar : ToolbarOverlay
    {
        private const string ID = "Toolnity Toolbar";
        
        public ToolnityToolbar() : base (
            SaveAllShortcutToolbar.ID,
            OnPlayToolbar.ID,
            SceneSelectorToolbar.ID,
            ToDoListSelectorToolbar.ID,
            CustomButtonsToolbar.ID,
            CameraShortcutsToolbar.ID,
            LightingToolbar.ID,
            InterestingGameObjectToolbar.ID,
            TeleportGameObjectToolbar.ID,
            FindMissingScriptsToolbar.ID,
            FavoritesToolbar.ID,
            ReplaceToolToolbar.ID,
            TransformRandomizerToolbar.ID,
            GameViewResolutionsToolbar.ID,
            SettingsToolbar.ID
            )
        {
        }
    }
}
#endif