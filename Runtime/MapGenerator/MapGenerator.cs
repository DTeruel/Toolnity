using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Toolnity
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private MapConfig mapConfig;
        [SerializeField] private bool clearAndGenerateOnPlay;

        private Dictionary<string, GameObject> rootGameObjects = new Dictionary<string, GameObject>(); 

        private void Start()
        {
            if (clearAndGenerateOnPlay)
            {
                ClearAndGenerate();
            }
        }
        
        public void ClearAndGenerate()
        {
            Clear();
            Generate();
        }
        
        public void Clear()
        {
            while(transform.childCount > 0)
            {
                #if UNITY_EDITOR
                    Undo.DestroyObjectImmediate(transform.GetChild(0).gameObject);
                #else
                    DestroyImmediate(transform.GetChild(0).gameObject);
                #endif
            }
        }
        
        public void Generate()
        {
            Debug.Log("[Map Generator] Generating map '" + mapConfig.Description + "'...");

            InitializeGeneration();
            
            for (var x = 0; x < mapConfig.MapSource.width; x++)
            {
                for (var y = 0; y < mapConfig.MapSource.height; y++)
                {
                    GenerateTile(x, y);
                }
            }
            
            Debug.Log("[Map Generator] Generation done!");
        }

        private void InitializeGeneration()
        {
            rootGameObjects.Clear();
            for (var i = 0; i < mapConfig.TokensConfig.AllTokens.Length; i++)
            {
                var token = mapConfig.TokensConfig.AllTokens[i];
                token.ColorToMatchV3 = new Vector3(token.ColorToMatch.r, token.ColorToMatch.g, token.ColorToMatch.b);
                token.ColorToMatchV3 = token.ColorToMatchV3.Round();

                var go = new GameObject(token.Description)
                {
                    transform =
                    {
                        parent = gameObject.transform
                    }
                };
                rootGameObjects.Add(token.Description + token.ColorToMatchV3, go);
            }
        }

        private void GenerateTile(int x, int y)
        {
            var pixelColor = mapConfig.MapSource.GetPixel(x, y);
            if (pixelColor.a < 1)
            {
                return;
            }

            var pixelColorV3 = new Vector3(pixelColor.r, pixelColor.g, pixelColor.b);
            pixelColorV3 = pixelColorV3.Round();

            var colorFound = false;
            for (var i = 0; i < mapConfig.TokensConfig.AllTokens.Length; i++)
            {
                var token = mapConfig.TokensConfig.AllTokens[i];
                if (token.ColorToMatchV3.Equals(pixelColorV3) && 
                    token.PrefabsToSpawn.Length > 0)
                {
                    colorFound = true;
                    if (token.probabilityOfSpawn > Random.Range(0f, 0.99f))
                    {
                        GenerateToken(token, x, y);
                    }
                }
            }

            if (!colorFound)
            {
                Debug.LogWarning("[Map Generator] Color not found: " + pixelColorV3);
                
                // for (var i = 0; i < mapConfig.TokensConfig.AllTokens.Length; i++)
                // {
                //     var token = mapConfig.TokensConfig.AllTokens[i];
                //     Debug.Log("[Map Generator] Color available: " + token.ColorToMatchV3);
                // }
            }
        }
        
        private void GenerateToken(MapTokensConfig.TokenConfig token, int x, int y)
        {
            var position = new Vector2(x * mapConfig.ScaleMultiplier.x, y * mapConfig.ScaleMultiplier.y);

            switch (token.SpawnType)
            {
                case MapTokensConfig.TypeOfSpawn.SpawnAll:
                    for (var j = 0; j < token.PrefabsToSpawn.Length; j++)
                    {
                        SpawnObject(token.Description + token.ColorToMatchV3, token.PrefabsToSpawn[j], position);
                    }
                    break;
                case MapTokensConfig.TypeOfSpawn.SpawnJustOneRandomly:
                    var randomIndex = Random.Range(0, token.PrefabsToSpawn.Length);
                    SpawnObject(token.Description + token.ColorToMatchV3, token.PrefabsToSpawn[randomIndex], position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SpawnObject(string tokenDescription, GameObject prefabToSpawn, Vector2 position)
        {
            //Debug.Log("[Map Generator] Creating Object '" + prefabToSpawn.name + "'...");
            if (prefabToSpawn == null)
            {
                Debug.LogError("[Map Generator] Invalid Prefab assigned. Please, check your Scriptable Object MapTokenConfig.");
                return;
            }
            var newObject = Instantiate(prefabToSpawn, position, Quaternion.identity, rootGameObjects[tokenDescription].transform);
            #if UNITY_EDITOR
                Undo.RegisterCreatedObjectUndo(newObject, "Create object");
            #endif
        }
    }
}