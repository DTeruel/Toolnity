using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Toolnity.CustomButtons
{
    public partial class CustomButtonsMenu
    {
        private const string MAIN_FOLDER_NAME = "Main Menu";
        private const string STATIC_FOLDER_NAME = "- Statics -";

        public enum CustomButtonPositionNames
        {
            TopRight = 0,
            TopLeft = 1,
            BottomRight = 2,
            BottomLeft = 3
        }

        private class CustomButtonInstance
        {
            public readonly Button ButtonInstance;
            public readonly MonoBehaviour Mono;
            public readonly bool StaticFunction;

            private readonly MethodInfo methodGetName;

            public CustomButtonInstance(Button buttonInstance, MonoBehaviour mono, MethodInfo methodGetName)
            {
                ButtonInstance = buttonInstance;
                Mono = mono;
                StaticFunction = mono == null;
                this.methodGetName = methodGetName;
            }

            public void UpdateName()
            {
                if (methodGetName == null)
                {
                    return;
                }

                var buttonText = ButtonInstance.GetComponentInChildren<Text>();
                var nameFunction = methodGetName.Invoke(Mono, null);
                buttonText.text = nameFunction.ToString();
            }
        }

        private class CustomButtonFolder
        {
            public string Name;
            public readonly Button ButtonInstance;
            public readonly CustomButtonFolder ParentFolder;
            public readonly List<CustomButtonFolder> Subfolders = new();
            public readonly List<CustomButtonInstance> Functions = new();

            public CustomButtonFolder() { }

            public CustomButtonFolder(
                string name,
                Button buttonInstance,
                CustomButtonFolder parentFolder)
            {
                Name = name;
                ButtonInstance = buttonInstance;
                ParentFolder = parentFolder;
            }
        }
    }
}
