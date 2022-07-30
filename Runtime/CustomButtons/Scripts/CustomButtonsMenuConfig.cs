using UnityEngine;

namespace Toolnity
{
    [CreateAssetMenu(menuName = "Toolnity/Custom Buttons Menu", fileName = "Custom Buttons Menu")]
    public class CustomButtonsMenuConfig : ScriptableObject
    {
        public bool enabled = true;
        public CustomButtonsMenu.CustomButtonPositionNames position = CustomButtonsMenu.CustomButtonPositionNames.TopRight;
    }
}