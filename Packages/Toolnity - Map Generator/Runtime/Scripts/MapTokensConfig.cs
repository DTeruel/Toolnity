using System;
using UnityEngine;

namespace Toolnity.MapGenerator
{
    [CreateAssetMenu(fileName = "[Map Generator] Tokens Config", menuName = "Toolnity/Map Generator/Tokens Config")]
    public class MapTokensConfig : ScriptableObject
    {
        [Serializable]
        public enum TypeOfSpawn
        {
            SpawnAll,
            SpawnJustOneRandomly
        }
        
        [Serializable]
        public class TokenConfig
        {
            public string Description;
            public Color ColorToMatch;
            [NonSerialized] public Vector3 ColorToMatchV3;
            [Range(0f, 1f)] public float probabilityOfSpawn = 1f; 
            public TypeOfSpawn SpawnType;
            public GameObject[] PrefabsToSpawn;
        }

        public TokenConfig[] AllTokens;
    }
}
