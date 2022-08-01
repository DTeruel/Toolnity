using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Toolnity
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

            private readonly MethodInfo methodName;

            public CustomButtonInstance(Button buttonInstance, MonoBehaviour mono, MethodInfo methodName)
            {
                ButtonInstance = buttonInstance;
                Mono = mono;
                this.methodName = methodName;
            }

            public void UpdateName()
            {
                if (methodName == null)
                {
                    return;
                }

                var buttonText = ButtonInstance.GetComponentInChildren<Text>();
                var nameFunction = methodName.Invoke(Mono, null);
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