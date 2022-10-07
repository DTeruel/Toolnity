using UnityEngine;

namespace Toolnity.CustomButtons
{
    [CreateAssetMenu(menuName = "Toolnity/Custom Buttons Menu Config", fileName = "Custom Buttons Menu Config")]
    public class CustomButtonsMenuConfig : ScriptableObject
    {
        public bool enabled = true;
        public bool mainButtonVisible = true;
        public CustomButtonsMenu.CustomButtonPositionNames position = CustomButtonsMenu.CustomButtonPositionNames.TopRight;
    }
}