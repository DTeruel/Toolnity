#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Toolnity.ToDoList
{
    [CustomEditor(typeof(ToDoList))]
    public class ToDoListDrawer : Editor
    {
        private const float DEFAULT_VERTICAL_OFFSET = 15;
        private const float MIDDLE_VERTICAL_OFFSET = 10;
        private const float TASKS_BUTTON_WIDTH = 70;
        private const float SIDES_WIDTH_SPACE = 10;
        private const float OPTIONS_HORIZONTAL_OFFSET = 20;
        private const float TASK_VERTICAL_OFFSET = 4;
        private const float NEW_TASK_HEIGHT = 35;
        private const float SPACE_WIDTH_PERCENTAGE = 0.1f;

        private GUIStyle titleCenteredLabel;
        private GUIStyle fieldCentered;
        private GUIStyle centeredDropdown;
        private GUIStyle textAreaCentered;
        private GUIStyle buttonDeleteParameter;
        private bool createCommonVars;

        #region MAIN
        public void OnEnable()
        {
            createCommonVars = true;
        }

        public override void OnInspectorGUI()
        {
            if (createCommonVars)
            {
                CreateCommonVars();
            }
            var toDoList = (ToDoList)target;
            DrawTasks("TASKS", toDoList.tasks);
            DrawNewTaskButton();
            DrawCompletedTasks();
            DrawConfiguration();

            EditorUtility.SetDirty(target);
        }
        #endregion
        
        #region TASKS
        private static GUIStyle[] rectangleColor;

        private void DrawTasks(string title, IReadOnlyList<ToDoElement> list, bool taskCompleted = false)
        {
            var toDoList = (ToDoList)target;
            
            EditorGUILayout.Space(DEFAULT_VERTICAL_OFFSET);
            EditorGUILayout.LabelField(title, titleCenteredLabel);
            
            if (!taskCompleted)
            {
                DrawFilter();
            }
            
            EditorGUILayout.Space(DEFAULT_VERTICAL_OFFSET);
            
            for (var i = 0; i < list.Count; i++)
            {
                if (toDoList.showFilters)
                {
                    if (toDoList.filter1Enabled)
                    {
                        if (list[i].parameter1Id != toDoList.filterIndexParameter1)
                        {
                            continue;
                        }
                    }
                    if (toDoList.filter2Enabled)
                    {
                        if (list[i].parameter2Id != toDoList.filterIndexParameter2)
                        {
                            continue;
                        }
                    }
                }

                var rectangleColorIndex = list[i].parameter1Id % rectangleColor.Length;
                EditorGUILayout.BeginVertical(rectangleColor[rectangleColorIndex]);
                // Blank Line
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(MIDDLE_VERTICAL_OFFSET);
                EditorGUILayout.EndHorizontal();
                
                // Main - START
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(SIDES_WIDTH_SPACE);
                
                // Main - Up & Down
                DrawUpDownButtons(i, list.Count);
                
                // Main - Description
                GUILayout.FlexibleSpace();
                list[i].description = DrawDescription(list[i].description);
                GUILayout.FlexibleSpace();
                
                // Main - Delete & Complete
                if (DrawDeleteCompleteAndRecoverButtons(i, taskCompleted))
                {
                    return;
                }
                
                // Main - END
                GUILayout.Space(SIDES_WIDTH_SPACE);
                EditorGUILayout.EndHorizontal();

                // Blank Line
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(MIDDLE_VERTICAL_OFFSET);
                EditorGUILayout.EndHorizontal();
                
                // Parameters
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(Screen.width * SPACE_WIDTH_PERCENTAGE);
                list[i].parameter1Id = DrawDropdown(list[i].parameter1Id, toDoList.parameters1.ToArray());
                GUILayout.Space(OPTIONS_HORIZONTAL_OFFSET);
                list[i].parameter2Id = DrawDropdown(list[i].parameter2Id, toDoList.parameters2.ToArray());
                GUILayout.Space(Screen.width * SPACE_WIDTH_PERCENTAGE);
                EditorGUILayout.EndHorizontal();

                // Blank Line
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(MIDDLE_VERTICAL_OFFSET);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space(TASK_VERTICAL_OFFSET);
            }
        }
        
        private void DrawFilter()
        {
            var toDoList = (ToDoList)target;
            EditorGUILayout.Space(MIDDLE_VERTICAL_OFFSET);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            toDoList.showFilters = GUILayout.Toggle(toDoList.showFilters, " FILTERS");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (toDoList.showFilters)
            {
                EditorGUILayout.Space(DEFAULT_VERTICAL_OFFSET);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(Screen.width * SPACE_WIDTH_PERCENTAGE);
                toDoList.filter1Enabled = GUILayout.Toggle(toDoList.filter1Enabled, "");
                toDoList.filterIndexParameter1 = DrawDropdown(toDoList.filterIndexParameter1, toDoList.parameters1.ToArray());
                
                GUILayout.Space(OPTIONS_HORIZONTAL_OFFSET);
                toDoList.filter2Enabled = GUILayout.Toggle(toDoList.filter2Enabled, "");
                toDoList.filterIndexParameter2 = DrawDropdown(toDoList.filterIndexParameter2, toDoList.parameters2.ToArray());
                
                GUILayout.Space(OPTIONS_HORIZONTAL_OFFSET);
                toDoList.showCompletedTasks = GUILayout.Toggle(toDoList.showCompletedTasks,
                    " COMPLETED TASKS");
                
                GUILayout.Space(Screen.width * SPACE_WIDTH_PERCENTAGE);
                EditorGUILayout.EndHorizontal();
            }
        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            var pix = new Color[width * height];

            for (var i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        private void DrawUpDownButtons(int i, int listLength)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (i == 0)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Up", GUILayout.Width(TASKS_BUTTON_WIDTH)))
            {
                SwitchElementsInList(i, i - 1);
            }
            GUI.enabled = true;

            if (i == listLength - 1)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Down", GUILayout.Width(TASKS_BUTTON_WIDTH)))
            {
                SwitchElementsInList(i, i + 1);
            }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        private string DrawDescription(string description)
        {
            textAreaCentered.fixedWidth = Screen.width * 0.5f;

            return GUILayout.TextArea(description, textAreaCentered);
        }

        private bool DrawDeleteCompleteAndRecoverButtons(int i, bool taskCompleted)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("DELETE", GUILayout.Width(TASKS_BUTTON_WIDTH)))
            {
                var toDoList = (ToDoList)target;
                toDoList.tasks.RemoveAt(i);
                return true;
            }

            if (taskCompleted)
            {
                if (GUILayout.Button("Recover", GUILayout.Width(TASKS_BUTTON_WIDTH)))
                {
                    var toDoList = (ToDoList)target;
                    toDoList.tasks.Add(toDoList.completedTasks[i]);
                    toDoList.completedTasks.RemoveAt(i);
                    return true;
                }
            }
            else
            {
                if (GUILayout.Button("Complete", GUILayout.Width(TASKS_BUTTON_WIDTH)))
                {
                    var toDoList = (ToDoList)target;
                    toDoList.completedTasks.Add(toDoList.tasks[i]);
                    toDoList.tasks.RemoveAt(i);
                    return true;
                }
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            return false;
        }

        private void DrawNewTaskButton()
        {
            EditorGUILayout.Space(MIDDLE_VERTICAL_OFFSET);
            
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
        }
        #endregion

        #region COMPLETED TASKS

        private void DrawCompletedTasks()
        {
            var toDoList = (ToDoList)target;

            if (toDoList.showFilters && toDoList.showCompletedTasks && toDoList.completedTasks.Count > 0)
            {
                EditorGUILayout.Space(DEFAULT_VERTICAL_OFFSET);
                DrawTasks("COMPLETED TASKS", toDoList.completedTasks, true);
            }
        }
        #endregion

        #region ASSIGNERS & PRIORITIES

        private void DrawConfiguration()
        {
            var toDoList = (ToDoList)target;

            EditorGUILayout.Space(MIDDLE_VERTICAL_OFFSET);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            toDoList.configurationEnabled = GUILayout.Toggle(toDoList.configurationEnabled,
                " CONFIGURATION");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(DEFAULT_VERTICAL_OFFSET);

            if (toDoList.configurationEnabled)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                DrawConfigList("PARAMETER #1", toDoList.parameters1, true);
                GUILayout.Space(OPTIONS_HORIZONTAL_OFFSET);
                DrawConfigList("PARAMETER #2", toDoList.parameters2);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawConfigList(string title, IList<string> list, bool changeColors = false)
        {
            var itemToRemove = -1;
            EditorGUILayout.BeginVertical();
            GUILayout.Label(title, titleCenteredLabel);
            GUILayout.Space(DEFAULT_VERTICAL_OFFSET);
            for (var i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (changeColors)
                {
                    var rectangleColorIndex = i % rectangleColor.Length;
                    var originalBackground = fieldCentered.normal.background;
                    fieldCentered.normal.background = rectangleColor[rectangleColorIndex].normal.background;
                    list[i] = GUILayout.TextField(list[i], fieldCentered);
                    fieldCentered.normal.background = originalBackground;
                }
                else
                {
                    list[i] = GUILayout.TextField(list[i], fieldCentered);
                }
                if (GUILayout.Button("-", buttonDeleteParameter))
                {
                    itemToRemove = i;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                list.Add("");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (itemToRemove > -1)
            {
                list.RemoveAt(itemToRemove);
            }
        }
        #endregion

        #region UTILS
        private void CreateCommonVars()
        {
            titleCenteredLabel = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 15
            };

            fieldCentered = new GUIStyle(GUI.skin.GetStyle("TextField"))
            {
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true
            };

            centeredDropdown = new GUIStyle(GUI.skin.GetStyle("Popup"))
            {
                alignment = TextAnchor.MiddleCenter
            };

            textAreaCentered = new GUIStyle(GUI.skin.GetStyle("TextArea"))
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fixedHeight = 50
            };

            buttonDeleteParameter = new GUIStyle(GUI.skin.GetStyle("Button"))
            {
                fixedWidth = 20
            };
            
            CreateRectangleColors();

            createCommonVars = false;
        }

        private static void CreateRectangleColors()
        {
            rectangleColor = new []
            {
                new GUIStyle
                {
                    normal =
                    {
                        background = MakeTex(1, 1, new Color(0.1f, 0.2f, 0.1f))
                    }
                },
                new GUIStyle
                {
                    normal =
                    {
                        background = MakeTex(1, 1, new Color(0.2f, 0.1f, 0.1f))
                    }
                },
                new GUIStyle
                {
                    normal =
                    {
                        background = MakeTex(1, 1, new Color(0.1f, 0.1f, 0.2f))
                    }
                },
                new GUIStyle
                {
                    normal =
                    {
                        background = MakeTex(1, 1, new Color(0.2f, 0.2f, 0.1f))
                    }
                },
                new GUIStyle
                {
                    normal =
                    {
                        background = MakeTex(1, 1, new Color(0.0f, 0.2f, 0.2f))
                    }
                },
                new GUIStyle
                {
                    normal =
                    {
                        background = MakeTex(1, 1, new Color(0.2f, 0.0f, 0.2f))
                    }
                }
            };
        }

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