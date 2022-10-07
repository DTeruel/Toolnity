using UnityEngine;

namespace Toolnity.MapGenerator
{
    [CreateAssetMenu(fileName = "[Map Generator] Map Config", menuName = "Toolnity/Map Generator/Map Config")]
    public class MapConfig : ScriptableObject
    {
        public string Description;
        public MapTokensConfig TokensConfig;
        public Texture2D MapSource;
        public Vector2 ScaleMultiplier = Vector2.one;
    }
}