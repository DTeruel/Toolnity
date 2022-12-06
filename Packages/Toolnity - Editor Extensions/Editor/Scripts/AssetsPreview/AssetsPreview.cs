#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Toolnity.EditorExtensions
{
    [ExecuteInEditMode]
    public class AssetsPreview : MonoBehaviour
    {
        public enum FileType
        {
            Prefabs = 0,
            Materials = 1,
            Textures = 2,
            Models = 3,
            
            Count = 4
        }

        [SerializeField] public bool Recursive = true;
        [NonSerialized] public float Position;
        [NonSerialized] public float AutoRotateSpeed;
        [NonSerialized] public float Scale = 1f;
        [SerializeField] private bool UsingLitMaterial;
        
        [SerializeField] public string Path;
        [SerializeField][HideInInspector] private string fullPath;

        private readonly List<string> validFiles = new();
        private readonly List<string> invalidFiles = new();

        private readonly List<string> prefabs = new();
        private readonly List<string> materials = new();
        private readonly List<string> textures = new();
        private readonly List<string> models = new();

        private GameObject assetLoaded;
        private string currentAssetRelativePath;
        private Material defaultLitMaterial;
        private Material defaultUnlitMaterial;

        public void SelectNewPath()
        {
            CleanAll();
            
            fullPath = EditorUtility.OpenFolderPanel(
                "Select Path to preview", Application.dataPath, "");
            if (string.IsNullOrEmpty(fullPath))
            {
                Path = null;
            }
            else
            {
                Path = GetRelativePath(fullPath);
                var obj = AssetDatabase.LoadAssetAtPath(Path, typeof(Object));
                Debug.Log("[Assets Preview] Selected Path: " + Path, obj);

                RefreshFiles();
            }
        }

        private void CleanAll()
        {
            DestroyPreviousAssetPreview();

            validFiles.Clear();
            invalidFiles.Clear();
            prefabs.Clear();
            materials.Clear();
            textures.Clear();
            models.Clear();

            AutoRotateSpeed = 0f;
            Scale = 1f;
            Position = 0f;
        }

        public void RefreshFiles()
        {
            CleanAll();
            Debug.Log("[Assets Preview] Searching Files in: " + Path);
            SearchFiles(fullPath);

            Debug.Log("[Assets Preview] Assets Recognized: " + GetNumValidFiles());
            Debug.Log("[Assets Preview] Assets Ignored: " + GetNumInvalidFiles());
        }

        private void SearchFiles(string path)
        {
            if (Recursive)
            {
                var subdirectories = Directory.GetDirectories(path);
                foreach(var directory in subdirectories)
                {
                    SearchFiles(directory);
                }
            }

            var files = Directory.GetFiles(path);
            AddFiles(files);
        }

        private void AddFiles(IEnumerable<string> files)
        {
            foreach(var file in files)
            {
                if (file.EndsWith(".prefab"))
                {
                    validFiles.Add(file);
                    prefabs.Add(file);
                    ShowReadingFile(file);
                }
                else if (file.EndsWith(".mat"))
                {
                    validFiles.Add(file);
                    materials.Add(file);
                    ShowReadingFile(file);
                }
                else if (file.EndsWith(".png") ||
                         file.EndsWith(".tiff") ||
                         file.EndsWith(".jpg") ||
                         file.EndsWith(".jpeg") ||
                         file.EndsWith(".psd") ||
                         file.EndsWith(".psb"))
                {
                    validFiles.Add(file);
                    textures.Add(file);
                    ShowReadingFile(file);
                }
                else if (file.EndsWith(".fbx") ||
                         file.EndsWith(".blend"))
                {
                    validFiles.Add(file);
                    models.Add(file);
                    ShowReadingFile(file);
                }
                else if (file.EndsWith(".meta") ||
                         file.EndsWith(".asset") ||
                         file.EndsWith(".unity") ||
                         file.EndsWith(".lighting"))
                {
                    // Ignoring unity objects
                }
                else
                {
                    invalidFiles.Add(file);
                    ShowReadingFile(file, false);
                }
            }
        }

        private static void ShowReadingFile(string file, bool isValid = true)
        {
            var relativePath = GetRelativePath(file);
            var obj = AssetDatabase.LoadAssetAtPath(relativePath, typeof(Object));
            if (isValid)
            {
                Debug.Log("[Assets Preview] Reading file: " + relativePath, obj);
            }
            else
            {
                Debug.Log("[Assets Preview] Not recognized file: " + relativePath, obj);
            }
        }

        public static string GetFileTypeName(int i)
        {
            var fileType = (FileType)i;
            return fileType.ToString();
        }

        public int GetFilesCount(int fileType)
        {
            switch (fileType)
            {
                case 0:
                    return prefabs.Count;
                case 1:
                    return materials.Count;
                case 2:
                    return textures.Count;
                case 3:
                    return models.Count;
            }
            
            return 0;
        }

        private int GetNumValidFiles()
        {
            return validFiles.Count;
        }

        private int GetNumInvalidFiles()
        {
            return invalidFiles.Count;
        }

        public void ShowFile(int fileType, int fileIndex)
        {
            DestroyPreviousAssetPreview();
            
            switch (fileType)
            {
                case 0:
                    ShowPrefab(fileIndex);
                    break;
                case 1:
                    ShowMaterial(fileIndex);
                    break;
                case 2:
                    ShowTexture(fileIndex);
                    break;
                case 3:
                    ShowModel(fileIndex);
                    break;
            }
        }

        private void DestroyPreviousAssetPreview()
        {
            if (assetLoaded == null)
            {
                return;
            }
            
            DestroyImmediate(assetLoaded);
        }
        
        private void ShowPrefab(int fileIndex)
        {
            currentAssetRelativePath = GetRelativePath(prefabs[fileIndex]);
            var prefab = AssetDatabase.LoadAssetAtPath(currentAssetRelativePath, typeof(GameObject)) as GameObject;

            if (prefab != null)
            {
                assetLoaded = Instantiate(prefab);
                assetLoaded.SetActive(true);
                assetLoaded.hideFlags = HideFlags.HideAndDontSave;
                SetThisAsParent();
            }
            else
            {
                Debug.LogError("[Assets Preview] Error loading prefab at path: " + currentAssetRelativePath);
            }
        }
        
        private void ShowMaterial(int fileIndex)
        {
            currentAssetRelativePath = GetRelativePath(materials[fileIndex]);
            var material = AssetDatabase.LoadAssetAtPath(currentAssetRelativePath, typeof(Material)) as Material;

            if (material != null)
            {
                assetLoaded = GameObject.CreatePrimitive(PrimitiveType.Plane);
                assetLoaded.name = material.name;
                assetLoaded.transform.localRotation = Quaternion.Euler(90, 0, -180);
                
                var meshRenderer = assetLoaded.GetComponent<MeshRenderer>();
                meshRenderer.material = material;
                
                assetLoaded.hideFlags = HideFlags.HideAndDontSave;
                SetThisAsParent();
            }
            else
            {
                Debug.LogError("[Assets Preview] Error loading material at path: " + currentAssetRelativePath);
            }
        }
        
        private void ShowTexture(int fileIndex)
        {
            currentAssetRelativePath = GetRelativePath(textures[fileIndex]);
            var texture = AssetDatabase.LoadAssetAtPath(currentAssetRelativePath, typeof(Texture)) as Texture;

            if (texture != null)
            {
                assetLoaded = GameObject.CreatePrimitive(PrimitiveType.Plane);
                assetLoaded.name = texture.name;
                assetLoaded.transform.localRotation = Quaternion.Euler(90, 0, -180);
                
                var meshRenderer = assetLoaded.GetComponent<MeshRenderer>();
                UpdateMaterial();
                meshRenderer.sharedMaterial.mainTexture = texture;
                
                assetLoaded.hideFlags = HideFlags.HideAndDontSave;
                SetThisAsParent();
            }
            else
            {
                Debug.LogError("[Assets Preview] Error loading material at path: " + currentAssetRelativePath);
            }
        }
        
        private void ShowModel(int fileIndex)
        {
            currentAssetRelativePath = GetRelativePath(models[fileIndex]);
            var model = AssetDatabase.LoadAssetAtPath(currentAssetRelativePath, typeof(Mesh)) as Mesh;

            if (model != null)
            {
                assetLoaded = new GameObject
                {
                    name = model.name
                };
                var meshFilter = assetLoaded.AddComponent<MeshFilter>();
                meshFilter.mesh = model;
                
                var meshRenderer = assetLoaded.AddComponent<MeshRenderer>();
                UpdateMaterial();
                meshRenderer.sharedMaterial.mainTexture = null;
                
                assetLoaded.hideFlags = HideFlags.HideAndDontSave;
                SetThisAsParent();
            }
            else
            {
                Debug.LogError("[Assets Preview] Error loading model at path: " + currentAssetRelativePath);
            }
        }

        public void UseLitMaterial(bool value)
        {
            UsingLitMaterial = value;
            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            if (!HasAssetLoaded())
            {
                return;
            }
            
            var meshRenderer = assetLoaded.GetComponent<MeshRenderer>();
            Texture texture = null;
            if (meshRenderer.sharedMaterial != null)
            {
                texture = meshRenderer.sharedMaterial.mainTexture;
            }

            var materialLit = GetDefaultLitMaterial();
            materialLit.mainTexture = null;
            var materialUnit = GetDefaultUnlitMaterial();
            materialUnit.mainTexture = null;
            
            if (UsingLitMaterial)
            {
                meshRenderer.material = materialLit;
            }
            else
            {
                meshRenderer.material = materialUnit;
            }
            if (texture != null)
            {
                meshRenderer.sharedMaterial.mainTexture = texture;
            }
        }

        private Material GetDefaultLitMaterial()
        {
            if (defaultLitMaterial != null)
            {
                return defaultLitMaterial;
            }

            var primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
            var meshRenderer = primitive.GetComponent<MeshRenderer>();
            defaultLitMaterial = meshRenderer.sharedMaterial;
            
            DestroyImmediate(primitive);
            
            return defaultLitMaterial;
        }
        
        private Material GetDefaultUnlitMaterial()
        {
            if (defaultUnlitMaterial != null)
            {
                return defaultUnlitMaterial;
            }

            defaultUnlitMaterial = new Material(Shader.Find("Unlit/Texture"));
            
            return defaultUnlitMaterial;
        }

        private static string GetRelativePath(string absolutePath)
        {
            const string assetFolder = "/Assets";
            var index = absolutePath.IndexOf(assetFolder, StringComparison.Ordinal);
            if (index >= 0) 
            {
                return absolutePath.Substring(index + 1);
            }
            else
            {
                Debug.LogWarning("[Assets Preview] 'Assets' folder not found!");
            }
            
            return "Assets/" + absolutePath;
        }

        public string GetCurrentAssetName()
        {
            if (HasAssetLoaded())
            {
                return assetLoaded.name;
            }
            
            return "";
        }

        public void SelectPathInProjectWindow()
        {
            DestroyPreviousAssetPreview();
            
            var obj = AssetDatabase.LoadAssetAtPath(currentAssetRelativePath, typeof(Object));
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        public bool HasAssetLoaded()
        {
            return assetLoaded != null;
        }

        public void SetThisAsParent()
        {
            if (!HasAssetLoaded())
            {
                return;
            }
            
            SetParent(transform);
        }

        public void SetMainCameraAsParent()
        {
            if (!HasAssetLoaded())
            {
                return;
            }

            var cameraMain = Camera.main; 
            if (cameraMain != null && cameraMain.gameObject != null)
            {
                SetParent(cameraMain.gameObject.transform);
            }
            else
            {
                SetParent(transform);
            }
        }

        private void SetParent(Transform parentTransform)
        {
            assetLoaded.transform.SetParent(parentTransform);
            assetLoaded.transform.localPosition = Vector3.zero;
        }
        
        private void OnEnable() 
        {
            EditorApplication.update += OnUpdate;
        }

        private void OnDisable() 
        {
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate()
        {
            if (Application.isPlaying || !HasAssetLoaded())
            {
                return;
            }

            assetLoaded.transform.Rotate(Vector3.up, AutoRotateSpeed);
            assetLoaded.transform.localScale = Vector3.one * Scale;
            var newPos = assetLoaded.transform.localPosition;
            newPos.z = Position;
            assetLoaded.transform.localPosition = newPos;
            
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }
    }
}
#endif