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
                EditorUtility.SetDirty(this);
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
                EditorUtility.SetDirty(this);
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
                EditorUtility.SetDirty(this);
            }
        }
    }
}