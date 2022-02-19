#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;

namespace Toolnity
{
    [Overlay(typeof(SceneView), "Toolnity", "Toolnity", true)]
    public class ToolnityToolbar : ToolbarOverlay
    {
        public ToolnityToolbar() : base (
            SaveAllShortcutToolbar.ID,
            OnPlayToolbar.ID,
            SceneSelectorToolbar.ID,
            ToDoListSelectorToolbar.ID,
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