using UnityEditor;
using UnityEngine;

namespace Toolnity.CustomButtons
{
    [CreateAssetMenu(menuName = "Toolnity/Custom Buttons Menu Config", fileName = "Custom Buttons Menu Config")]
    public class CustomButtonsMenuConfig : ScriptableObject
    {
        [SerializeField]
        private bool enabled = true;
        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }
        [SerializeField]
        private bool mainButtonVisible = true;
        public bool MainButtonVisible
        {
            get => mainButtonVisible;
            set
            {
                mainButtonVisible = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }
        [SerializeField]
        private CustomButtonsMenu.CustomButtonPositionNames position = CustomButtonsMenu.CustomButtonPositionNames.TopRight;
        public CustomButtonsMenu.CustomButtonPositionNames Position
        {
            get => position;
            set
            {
                position = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }
    }
}