#if UNITY_EDITOR
// Original code from https://github.com/nicoplv/smart-favorites

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolnity
{
    public class FavoritesPanel : EditorWindow
    {
        #region Variables

        private static FavoritesPanel favoritesPanelEditorWindow;
        private Vector2 scrollViewPosition = Vector2.zero;

        private Object lastObjectSelected ;
        private double lastObjectSelectedAt;
        private const double LastObjectSelectedTickOpen = 0.5f;
        private const double LastObjectSelectedTickPing = 2f;

        private double nextUpdate;
        private const double UpdateTick = 0.15f;

        private static readonly Color SelectNoPro = new Color(0.55f, 0.55f, 0.55f); 
        private static readonly Color SelectPro = new Color(0.3f, 0.3f, 0.3f);

        private static FavoritesAsset favoritesAsset;
        private static FavoritesAsset FavoritesAsset { get { if (!favoritesAsset) CheckFavoriteAsset(); return favoritesAsset; } }
        private ReorderableList reorderableList;

        private bool guiStyleDefined;
        private GUIStyle toolbarPopupGuiStyle;
        private GUIStyle reorderableListLabelGuiStyle;
        private GUIContent editButtonGuiStyle;
        private GUIContent plusButtonGuiStyle;
        private GUIContent minusButtonGuiStyle;

        private bool editNameList;

        #endregion

        #region Menu Item Methods

        [MenuItem("Tools/Toolnity/Favorites Panel", priority = 1500)]
        public static void ShowWindow()
        {
            favoritesPanelEditorWindow = GetWindow<FavoritesPanel>("Favorites");
            favoritesPanelEditorWindow.titleContent = new GUIContent("Favorites", EditorGUIUtility.IconContent("d_Favorite").image);
            favoritesPanelEditorWindow.Show();
        }

        [MenuItem("Assets/Add or Remove to Favorites %&F", false, priority = -10)]
        public static void AddRemoveToFavorite()
        {
            if (Selection.activeObject == null)
            {
                return;
            }
            
            if (!favoritesPanelEditorWindow)
            {
                ShowWindow();
            }

            CheckObject(Selection.objects, out var addObjects, out var removeObjects);

            if (addObjects.Count > 0)
            {
                FavoritesAsset.AddToCurrentList(addObjects);
            }
            else
            {
                FavoritesAsset.RemoveFromCurrentList(removeObjects);
            }
            favoritesPanelEditorWindow.Repaint();
        }

        private static void CheckObject(IEnumerable<Object> objects, out List<Object> addObjects, out List<Object> removeObjects)
        {
            addObjects = new List<Object>();
            removeObjects = new List<Object>();
            foreach (var iObject in objects)
            {
                var assetPath = AssetDatabase.GetAssetPath(iObject);
                if (assetPath == "" || AssetDatabase.LoadAssetAtPath<Object>(assetPath).GetInstanceID() != iObject.GetInstanceID())
                {
                    return;
                }

                if (!FavoritesAsset.CurrentList.Contains(iObject))
                {
                    addObjects.Add(iObject);
                }
                else
                {
                    removeObjects.Add(iObject);
                }
            }
        }

        private static void AddToFavoriteDrop(IEnumerable<Object> objects)
        {
            if (!favoritesPanelEditorWindow)
            {
                ShowWindow();
            }

            CheckObject(objects, out var addObjects, out _);

            if (addObjects.Count > 0)
            {
                FavoritesAsset.AddToCurrentList(addObjects);
            }
        }

        [MenuItem("Assets/Add or Remove to Favorites %&F", true, priority = -10)]
        public static bool AddRemoveToFavoriteValidate()
        {
            return Selection.activeObject != null;

        }

        private static void RemoveFavorite(object favObject)
        {
            var bObject = (Object)favObject;
            if (bObject && FavoritesAsset.CurrentList.Contains(bObject))
            {
                FavoritesAsset.RemoveFromCurrentList(bObject);
            }
        }

        private static void ClearFavorite()
        {
            if (EditorUtility.DisplayDialog("Clear the list \"" + FavoritesAsset.CurrentList.Name + "\"?", "Are you sure you want delete all the Favorites of the list \"" + FavoritesAsset.CurrentList.Name + "\"?", "Yes", "No"))
            {
                FavoritesAsset.ClearCurrentList();
            }
        }

        #endregion

        #region Window Methods

        public void OnEnable()
        {
            reorderableList = new ReorderableList(null, typeof(GameObject), false, false, false, false)
            {
                showDefaultBackground = false, 
                headerHeight = 0f, 
                footerHeight = 0f, 
                drawElementCallback = DrawFavoriteElement
            };
            CheckFavoriteAsset();
            guiStyleDefined = false;
        }

        private static void CheckFavoriteAsset()
        {
            if (favoritesAsset)
            {
                return;
            }
            
            var favoriteAssetFound = AssetDatabase.FindAssets("t:FavoritesAsset");
            if (favoriteAssetFound.Length > 0)
            {
                favoritesAsset = AssetDatabase.LoadAssetAtPath<FavoritesAsset>(AssetDatabase.GUIDToAssetPath(favoriteAssetFound[0]));
            }

            if (!favoritesAsset)
            {
                favoritesAsset = CreateInstance<FavoritesAsset>();
                AssetDatabase.CreateAsset(favoritesAsset, "Assets/FavoritesAsset.asset");
                favoritesAsset.AddList();
                AssetDatabase.SaveAssets();
            }
        }
        
        private void DrawFavoriteElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var currentObject = FavoritesAsset.CurrentList.Get(index);
            if (!currentObject)
            {
                return;
            }

            var iconRect = new Rect(rect);
            iconRect.y += 1f;
            iconRect.height -= 4f;
            iconRect.width = iconRect.height;

            var labelRect = new Rect(rect);
            labelRect.y += 2f;
            labelRect.height -= 4f;
            labelRect.x += iconRect.width;
            labelRect.width -= iconRect.width;

            var backgroundRect = new Rect(rect)
            {
                x = 0, 
                width = position.width
            };

            if (Selection.objects.Contains(currentObject))
            {
                EditorGUI.DrawRect(backgroundRect, EditorPrefs.GetInt("UserSkin") == 1 ? SelectPro : SelectNoPro);
            }

            GUI.DrawTexture(iconRect, AssetPreview.GetMiniThumbnail(currentObject), ScaleMode.ScaleToFit, true);
            EditorGUI.LabelField(labelRect, currentObject.name, reorderableListLabelGuiStyle);

            if (Event.current.isMouse)
            {
                switch (Event.current.type)
                {
                    case EventType.MouseUp when Event.current.button == 0 && rect.Contains(Event.current.mousePosition):
                    {
                        Selection.activeObject = currentObject;
                        if (lastObjectSelected == currentObject)
                        {
                            if (lastObjectSelectedAt + LastObjectSelectedTickOpen > EditorApplication.timeSinceStartup)
                            {
                                AssetDatabase.OpenAsset(currentObject);
                            }
                            else if (lastObjectSelectedAt + LastObjectSelectedTickPing > EditorApplication.timeSinceStartup)
                            {
                                EditorGUIUtility.PingObject(currentObject);
                            }
                        }
                        lastObjectSelected = currentObject;
                        lastObjectSelectedAt = EditorApplication.timeSinceStartup;
                        Event.current.Use();
                        break;
                    }

                    case EventType.MouseDown when Event.current.button == 0 && rect.Contains(Event.current.mousePosition):
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.SetGenericData("favorite", currentObject);
                        DragAndDrop.objectReferences = new [] { currentObject };
                        Event.current.Use();
                        break;
                    case EventType.MouseDrag when Event.current.button == 0 && (Object)(DragAndDrop.GetGenericData("favorite")) == currentObject:
                        DragAndDrop.StartDrag("Drag favorite");
                        Event.current.Use();
                        break;
                    case EventType.ContextClick when rect.Contains(Event.current.mousePosition):
                        ShowGenericMenu(currentObject);
                        Event.current.Use();
                        break;
                }
            }
        }
        
        private static void ShowGenericMenu(Object menuObject = null)
        {
            var genericMenu = new GenericMenu();
            if (menuObject)
            {
                genericMenu.AddItem(new GUIContent("Remove"), false, RemoveFavorite, menuObject);
            }
            else
            {
                genericMenu.AddDisabledItem(new GUIContent("Remove"));
            }
            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent("Clear All"), false, ClearFavorite);
            genericMenu.ShowAsContext();
        }

        public void OnLostFocus()
        {
            if (editNameList)
            {
                editNameList = false;
            }
        }
        
        public void Update()
        {
            if (EditorApplication.timeSinceStartup > nextUpdate)
            {
                nextUpdate = EditorApplication.timeSinceStartup + UpdateTick;
                Repaint();
            }
        }

        private void CheckGUIStyles()
        {
            if (guiStyleDefined)
            {
                return;
            }
            
            guiStyleDefined = true;
            toolbarPopupGuiStyle = new GUIStyle(EditorStyles.toolbarPopup);
            toolbarPopupGuiStyle.alignment = TextAnchor.MiddleCenter;
            reorderableListLabelGuiStyle = new GUIStyle(EditorStyles.label);
            reorderableListLabelGuiStyle.focused.textColor = reorderableListLabelGuiStyle.normal.textColor;
            editButtonGuiStyle = new GUIContent(EditorGUIUtility.IconContent("editicon.sml").image);
            plusButtonGuiStyle = new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus").image);
            minusButtonGuiStyle = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus").image);
        }
        
        public void OnGUI()
        {
            CheckGUIStyles();
            
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button(editButtonGuiStyle, GUILayout.ExpandWidth(false)))
            {
                ButtonEditFavoriteList();
            }

            if (editNameList)
            {
                GUI.SetNextControlName("EditNameList");
                FavoritesAsset.SetCurrentListName(
                    EditorGUILayout.TextField(FavoritesAsset.CurrentList.Name, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true)));
                EditorGUI.FocusTextInControl("EditNameList");
            }
            else
            {
                FavoritesAsset.CurrentListIndex = EditorGUILayout.Popup(FavoritesAsset.CurrentListIndex, FavoritesAsset.NameList(), toolbarPopupGuiStyle);
            }

            if (editNameList && ((Event.current.type == EventType.MouseUp && Event.current.button == 0) || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return))
            {
                editNameList = false;
            }

            EditorGUI.BeginDisabledGroup(editNameList);
            if (GUILayout.Button(plusButtonGuiStyle, GUILayout.ExpandWidth(false)))
            {
                ButtonAddFavoriteList();
            }
            EditorGUI.BeginDisabledGroup(FavoritesAsset.FavoriteLists.Count <= 1);
            if (GUILayout.Button(minusButtonGuiStyle, GUILayout.ExpandWidth(false)))
            {
                ButtonRemoveFavoriteList();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();

            var mouseOnWindow = Event.current.mousePosition.x >= 0 && Event.current.mousePosition.x <= position.width && Event.current.mousePosition.y >= 20 && Event.current.mousePosition.y <= position.height;

            switch (Event.current.type)
            {
                case EventType.DragUpdated when mouseOnWindow:
                {
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                    break;
                }

                case EventType.DragPerform when mouseOnWindow:
                    DragAndDrop.AcceptDrag();

                    AddToFavoriteDrop(DragAndDrop.objectReferences);

                    DragAndDrop.PrepareStartDrag();
                    Event.current.Use();
                    break;
            }

            scrollViewPosition = GUILayout.BeginScrollView(scrollViewPosition);

            FavoritesAsset.CurrentList.Update();
            reorderableList.list = FavoritesAsset.CurrentList.Objects;
            reorderableList.DoLayoutList();

            GUILayout.EndScrollView();

            switch (Event.current.type)
            {
                case EventType.ContextClick:
                    ShowGenericMenu();
                    break;
                case EventType.MouseUp when Event.current.button == 0:
                    Selection.activeObject = null;
                    break;
            }

            GUILayout.EndVertical();
        }

        #endregion

        #region Methods
        private static void ButtonAddFavoriteList()
        {
            FavoritesAsset.AddList();
        }

        private static void ButtonRemoveFavoriteList()
        {
            if (EditorUtility.DisplayDialog("Remove the list \"" + FavoritesAsset.CurrentList.Name + "\"?", "Are you sure you want delete the list \"" + FavoritesAsset.CurrentList.Name + "\"?", "Yes", "No"))
            {
                FavoritesAsset.RemoveList(FavoritesAsset.CurrentListIndex);
            }
        }

        private void ButtonEditFavoriteList()
        {
            editNameList = !editNameList;
        }

        #endregion
    }
}
#endif