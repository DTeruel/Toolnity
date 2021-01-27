#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
    [CustomEditor(typeof(ToDoList))]
    public class ToDoListDrawer : Editor
    {
        private const float DEFAULT_VERTICAL_OFFSET = 15;
        private const float UP_DOWN_BUTTON_WIDTH = 50;
        private const float DELETE_BUTTON_WIDTH = 25;
        private const float DESCRIPTION_SCREEN_WIDTH_REDUCTION = 120;
        private const float LEFT_RIGHT_OPTIONS_OFFSET = 70;
        private const float OPTIONS_HORIZONTAL_OFFSET = 20;
        private const float TASK_VERTICAL_OFFSET = 20;
        private const float NEW_TASK_HEIGHT = 35;
        
        private GUIStyle titleCenteredLabel;
        private GUIStyle fieldCentered;
        private GUIStyle centeredDropdown;
        private GUIStyle toggleCentered;

        private void CheckCommonVars()
        {
            if (titleCenteredLabel == null)
            {
                titleCenteredLabel = GUI.skin.GetStyle("Label");
                titleCenteredLabel.alignment = TextAnchor.MiddleCenter;
                titleCenteredLabel.fontStyle = FontStyle.Bold;
                titleCenteredLabel.fontSize = 15;
            }
            
            if (fieldCentered == null)
            {
                fieldCentered = GUI.skin.GetStyle("TextField");
                fieldCentered.alignment = TextAnchor.MiddleCenter;
            }
            
            if (centeredDropdown == null)
            {
                centeredDropdown = GUI.skin.GetStyle("Popup");
                centeredDropdown.alignment = TextAnchor.MiddleCenter;
            }
            
            if (toggleCentered == null)
            {
                toggleCentered = GUI.skin.GetStyle("Toggle");
                toggleCentered.alignment = TextAnchor.MiddleCenter;
            }
        }

        public override void OnInspectorGUI()
        {
            CheckCommonVars();
            
            var toDoList = (ToDoList)target;
            DrawTasks("TASKS", toDoList.tasks);
            DrawNewTask();
            DrawCompletedTasks();
            DrawConfiguration();
        }

        #region TASKS
        private void DrawTasks(string title, IReadOnlyList<ToDoElement> list, bool taskCompleted = false)
        {
            var toDoList = (ToDoList)target;
            
            EditorGUILayout.Space(DEFAULT_VERTICAL_OFFSET);
            EditorGUILayout.LabelField(title, titleCenteredLabel);
            EditorGUILayout.Space(DEFAULT_VERTICAL_OFFSET);

            for (var i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                DrawUpDownButtons(i, list.Count);
                GUILayout.FlexibleSpace();
                list[i].description = DrawDescription(list[i].description);
                GUILayout.FlexibleSpace();
                if (DrawDeleteButton(i))
                {
                    return;
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(LEFT_RIGHT_OPTIONS_OFFSET);
                list[i].assignerId = DrawDropdown(list[i].assignerId, toDoList.assigners.ToArray());
                GUILayout.Space(OPTIONS_HORIZONTAL_OFFSET);
                list[i].priorityId = DrawDropdown(list[i].priorityId, toDoList.priorities.ToArray());
                GUILayout.Space(OPTIONS_HORIZONTAL_OFFSET);
                DrawCompleteButton(i, taskCompleted);
                GUILayout.Space(LEFT_RIGHT_OPTIONS_OFFSET);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(TASK_VERTICAL_OFFSET);
            }
        }

        private void DrawUpDownButtons(int i, int listLength)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (i == 0)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Up", GUILayout.Width(UP_DOWN_BUTTON_WIDTH)))
            {
                SwitchElementsInList(i, i - 1);
            }
            GUI.enabled = true;
                
            if (i == listLength - 1)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Down", GUILayout.Width(UP_DOWN_BUTTON_WIDTH)))
            {
                SwitchElementsInList(i, i + 1);
            }
            GUI.enabled = true;
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        private static string DrawDescription(string description)
        {
            description = EditorGUILayout.TextArea(
                description, 
                GUILayout.Height(50), 
                GUILayout.Width(Screen.width - DESCRIPTION_SCREEN_WIDTH_REDUCTION));
            GUILayout.FlexibleSpace();

            return description;
        }
        
        private bool DrawDeleteButton(int i)
        {
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("X", GUILayout.Width(DELETE_BUTTON_WIDTH)))
            {
                var toDoList = (ToDoList)target;
                toDoList.tasks.RemoveAt(i);
                return true;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            return false;
        }

        private void DrawCompleteButton(int i, bool taskCompleted)
        {
            if (taskCompleted)
            {
                if (GUILayout.Button("Recover"))
                {
                    var toDoList = (ToDoList)target;
                    toDoList.tasks.Add(toDoList.completedTasks[i]);
                    toDoList.completedTasks.RemoveAt(i);
                }
            }
            else
            {
                if (GUILayout.Button("Complete"))
                {
                    var toDoList = (ToDoList)target;
                    toDoList.completedTasks.Add(toDoList.tasks[i]);
                    toDoList.tasks.RemoveAt(i);
                }
            }
        }
        
        private void DrawNewTask()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("NEW TASK",
                GUILayout.Height(NEW_TASK_HEIGHT), 
                GUILayout.Width(Screen.width / 2f)))
            {
                var toDoList = (ToDoList)target;
                toDoList.tasks.Add(new ToDoElement());
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(TASK_VERTICAL_OFFSET);
        }
        #endregion
        
        #region COMPLETED TASKS

        private void DrawCompletedTasks()
        {
            var toDoList = (ToDoList)target;
            
            EditorGUILayout.Space(TASK_VERTICAL_OFFSET);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            toDoList.showCompletedTasks = GUILayout.Toggle(toDoList.showCompletedTasks, 
                " SHOW COMPLETED TASKS");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(TASK_VERTICAL_OFFSET);

            if (toDoList.showCompletedTasks && toDoList.completedTasks.Count > 0)
            {
                DrawTasks("COMPLETED", toDoList.completedTasks, true);
            }
        }
        #endregion
        
        #region ASSIGNERS & PRIORITIES

        private void DrawConfiguration()
        {
            var toDoList = (ToDoList)target;
            
            EditorGUILayout.Space(TASK_VERTICAL_OFFSET);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            toDoList.configurationEnabled = GUILayout.Toggle(toDoList.configurationEnabled, 
                " CONFIGURATION");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(TASK_VERTICAL_OFFSET);

            if (toDoList.configurationEnabled)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                DrawConfigList("ASSIGNERS", toDoList.assigners);
                GUILayout.Space(OPTIONS_HORIZONTAL_OFFSET);
                DrawConfigList("PRIORITIES", toDoList.priorities);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawConfigList(string title, IList<string> list)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label(title, titleCenteredLabel);
            GUILayout.Space(DEFAULT_VERTICAL_OFFSET);
            for (var i = 0; i < list.Count; i++)
            {
                list[i] = GUILayout.TextField(list[i], fieldCentered);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-"))
            {
                list.RemoveAt(list.Count - 1);
            }
            if (GUILayout.Button("+"))
            {
                list.Add("");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        #endregion
        
        #region UTILS
        private void SwitchElementsInList(int originIndex, int destinyIndex)
        {
            var toDoList = (ToDoList)target;
            var elementBackup = toDoList.tasks[destinyIndex];
            toDoList.tasks[destinyIndex] = toDoList.tasks[originIndex];
            toDoList.tasks[originIndex] = elementBackup;
        }
        
        private int DrawDropdown(int id, string[] list)
        {
            return EditorGUILayout.Popup(id, list, centeredDropdown);
        }
        #endregion
    }
}
#endif