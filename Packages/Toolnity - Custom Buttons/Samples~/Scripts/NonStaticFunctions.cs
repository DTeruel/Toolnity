using UnityEngine;
using Toolnity.CustomButtons;

namespace Toolnity.Test
{
    public class NonStaticFunctions : MonoBehaviour
    {
        [CustomButton]
        private void BasicFunction()
        {
            Debug.Log("[Non Static Functions] Basic function called!");
        }
        
        [CustomButton(Name = "Function with static name")]
        private void FunctionWithStaticName()
        {
            Debug.Log("[Non Static Functions] Function with static name called!");
        }
        
        [CustomButton(NameFunction = nameof(GetDynamicName))]
        private void FunctionWithDynamicName()
        {
            Debug.Log("[Non Static Functions] Function with dynamic name called!");
        }

        private int nameIndex;
        private string GetDynamicName()
        {
            nameIndex++;
            return "Dynamic Name: " + nameIndex;
        }

        [CustomButton(Path = "Special", Name = "Close On Pressed", CloseMenuOnPressed = true)]
        private void FunctionToClose()
        {
            Debug.Log("[Non Static Functions] Function to close called!");
        }

        [CustomButton(PathAddGameObjectName = false)]
        private void FunctionOnlyClassName()
        {
            Debug.Log("[Non Static Functions] Function only class name called!");
        }

        [CustomButton(PathAddClassName = false)]
        private void FunctionOnlyGameObjectName()
        {
            Debug.Log("[Non Static Functions] Function only GameObject name called!");
        }

        [CustomButton(PathAddGameObjectName = false, PathAddClassName = false)]
        private void FunctionOnRoot()
        {
            Debug.Log("[Non Static Functions] Function on root called!");
        }

        [CustomButton(Shortcut = new []{KeyCode.LeftShift, KeyCode.Alpha1})]
        private void FunctionWithShortcut()
        {
            Debug.Log("[Non Static Functions] Function with shortcut called!");
        }
    }
}