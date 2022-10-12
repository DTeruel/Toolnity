using System.Collections.Generic;
using UnityEngine;
using Toolnity.CustomButtons;

namespace Toolnity.Test
{
    public class StaticFunctions : MonoBehaviour
    {
        [CustomButton]
        private static void BasicStaticFunction()
        {
            Debug.Log("[Static Functions] Basic static function called!");
        }
        
        [CustomButton(Name = "Static Function")]
        private static void FunctionWithName()
        {
            Debug.Log("[Static Functions] Static function with name called!");
        }
        
        [CustomButton(Name = "Static Function Without Class Name", PathAddClassName = false)]
        private static void FunctionWithoutClassName()
        {
            Debug.Log("[Static Functions] Static function without class name called!");
        }

        [CustomButton(Icon = "UnityLogoLarge")]
        private static void FunctionWithIcon()
        {
            Debug.Log("[Static Functions] Function with icon called!");
        }
        
        [CustomButton(ShowInRuntime = false, Shortcut = new []{KeyCode.LeftShift, KeyCode.Alpha3} /* This shortcut won't be called because is not in runtime*/)]
        private static void StaticFunctionHiddenInRuntime()
        {
            Debug.Log("[Static Functions] Function hidden in runtime called!");
        }

        [CustomButton(Shortcut = new []{KeyCode.LeftShift, KeyCode.Alpha2})]
        private static void FunctionWithShortcut()
        {
            Debug.Log("[Static Functions] Function with shortcut called!");
        }
    }
}