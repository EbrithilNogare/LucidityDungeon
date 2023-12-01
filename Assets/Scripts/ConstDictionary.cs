using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "ConstDictionary")]
    public class ConstDictionary : ScriptableObject
    {
        public Tile[] floorTiles;
        public Tile[] wallTiles;
        public Tile[] edgeUpTiles;
        public Tile[] edgeLeftTiles;
        public Tile[] edgeRightTiles;
        public Tile[] edgeUpLeftTiles;
        public Tile[] edgeUpRightTiles;
        public Tile[] tileToEmptyRoom;
        public Tile[] nothingTiles;
        public Sprite trader;
        public Sprite chestClose;
        public Sprite[] enemies;
    }
}

