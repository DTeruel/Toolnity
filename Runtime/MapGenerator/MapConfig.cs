using UnityEngine;

namespace Toolnity
{
    [CreateAssetMenu(fileName = "[MapEditor] Map Config", menuName = "Toolnity/Map Editor/Map Config")]
    public class MapConfig : ScriptableObject
    {
        public string Description;
        public MapTokensConfig TokensConfig;
        public Texture2D MapSource;
        public Vector2 ScaleMultiplier = Vector2.one;
    }
}