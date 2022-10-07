#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Toolnity.HierarchyWindowExtensions
{
    internal class ColorPickerWindow : EditorWindow
    {
        public Color color = Color.green;

        private Action<Color> setColorAction;

        public void Init(Color newColor, Action<Color> onSetColor)
        {
            color = newColor;
            setColorAction = onSetColor;
        }

        private void OnGUI()
        {
            var oldColor = color;
            color = EditorGUILayout.ColorField(color);

            if (oldColor != color)
            {
                setColorAction?.Invoke(color);
            }
        }
    }
}
#endif
