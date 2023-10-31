using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    public class GameEngineConnector : MonoBehaviour
    {
        [Header("Editable")]
        public GameObject player;
        public Tilemap tilemap;
        [Space(100)]
        [Header("From prefab")]
        public Tile[] floorTiles;
        public Tile[] wallTiles;
        public Tile[] edgeUpTiles;
        public Tile[] edgeLeftTiles;
        public Tile[] edgeRightTiles;
        public Tile[] edgeUpLeftTiles;
        public Tile[] edgeUpRightTiles;
        public Tile[] nothingTiles;
        public Sprite trader;
        public Sprite chestClose;
        public Sprite chestOpen;
        public GameObject enemyPrefab;
        public Sprite[] enemies;


        private GameEngine gameEngine;

        void Start()
        {
            Config config = new Config();
            gameEngine = new GameEngine(config);
        }

        private bool first = true;
        private void Update()
        {
            if (!first)
            {
                return;
            }
            first = false;

            for (int x = -15; x < 15; x++)
            {
                for (int y = -15; y < 15; y++)
                {
                    var pos = new Coordinate(x, y);
                    gameEngine.checkMapTile(pos);
                    RenderRoom(gameEngine.map[pos], pos);
                    RenderContent(gameEngine.map[pos], pos);
                }
            }


        }

        void RenderContent(MapTile mapTile, Coordinate coordinate)
        {
            if (mapTile.roomContent == MapRoomContent.Enemy)
            {
                GameObject newObj = Instantiate(enemyPrefab, new Vector3(coordinate.x * 8, coordinate.y * 8, 0), Quaternion.identity);
                newObj.GetComponent<SpriteRenderer>().sprite = enemies[0];
            }
            if (mapTile.roomContent == MapRoomContent.Treasure)
            {
                GameObject newObj = Instantiate(enemyPrefab, new Vector3(coordinate.x * 8, coordinate.y * 8, 0), Quaternion.identity);
                newObj.GetComponent<SpriteRenderer>().sprite = chestClose;
            }
            if (mapTile.roomContent == MapRoomContent.Trader)
            {
                GameObject newObj = Instantiate(enemyPrefab, new Vector3(coordinate.x * 8, coordinate.y * 8, 0), Quaternion.identity);
                newObj.GetComponent<SpriteRenderer>().sprite = trader;
            }

        }

        byte[,] defaultRoom = new byte[8, 8]{
            { 0, 0, 0, 1, 1, 0, 0, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 0, 1, 1, 0, 0, 0 },
        };

        void RenderRoom(MapTile mapTile, Coordinate coordinate)
        {
            var room = (byte[,])defaultRoom.Clone();

            if (!mapTile.entries.up)
            {
                if (mapTile.roomContent == MapRoomContent.Empty)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        room[0, x] = 0;
                        room[1, x] = 0;
                        room[2, x] = 0;
                    }
                }
                else
                {
                    room[0, 3] = 0;
                    room[0, 4] = 0;
                }
            }

            if (!mapTile.entries.left)
            {
                if (mapTile.roomContent == MapRoomContent.Empty)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        room[y, 0] = 0;
                        room[y, 1] = 0;
                        room[y, 2] = 0;
                    }
                }
                else
                {
                    room[3, 0] = 0;
                    room[4, 0] = 0;
                }
            }

            if (!mapTile.entries.down)
            {
                if (mapTile.roomContent == MapRoomContent.Empty)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        room[7, x] = 0;
                        room[6, x] = 0;
                        room[5, x] = 0;
                    }
                }
                else
                {
                    room[7, 3] = 0;
                    room[7, 4] = 0;
                }
            }

            if (!mapTile.entries.right)
            {
                if (mapTile.roomContent == MapRoomContent.Empty)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        room[y, 7] = 0;
                        room[y, 6] = 0;
                        room[y, 5] = 0;
                    }
                }
                else
                {
                    room[3, 7] = 0;
                    room[4, 7] = 0;
                }
            }


            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var randomIndex = Mathf.Abs(coordinate.x * 13 + coordinate.y * 29 + x * 79 + y * 53);
                    if (room[7 - y, x] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), floorTiles[randomIndex % floorTiles.Length]);
                        continue;
                    }

                    if (7 - y + 1 <= 7 && room[7 - y + 1, x] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), wallTiles[randomIndex % wallTiles.Length]);
                        continue;
                    }

                    if (x + 1 <= 7 && room[7 - y, x + 1] == 1 && 7 - y - 1 >= 0 && room[7 - y - 1, x] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), edgeUpRightTiles[randomIndex % edgeUpRightTiles.Length]);
                        continue;
                    }

                    if (x - 1 >= 0 && room[7 - y, x - 1] == 1 && 7 - y - 1 >= 0 && room[7 - y - 1, x] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), edgeUpLeftTiles[randomIndex % edgeUpLeftTiles.Length]);
                        continue;
                    }

                    if (7 - y - 1 >= 0 && room[7 - y - 1, x] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), edgeUpTiles[randomIndex % edgeUpTiles.Length]);
                        continue;
                    }

                    if ((x - 1 >= 0 && room[7 - y, x - 1] == 1) || (x - 1 >= 0 && 7 - y + 1 <= 7 && room[7 - y + 1, x - 1] == 1))
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), edgeLeftTiles[randomIndex % edgeLeftTiles.Length]);
                        continue;
                    }

                    if ((x + 1 <= 7 && room[7 - y, x + 1] == 1) || (x + 1 <= 7 && 7 - y + 1 <= 7 && room[7 - y + 1, x + 1] == 1))
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), edgeRightTiles[randomIndex % edgeRightTiles.Length]);
                        continue;
                    }

                    tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), nothingTiles[randomIndex % nothingTiles.Length]);
                }
            }
        }

    }
}
