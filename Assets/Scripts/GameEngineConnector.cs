using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    class GameEngineConnector : MonoBehaviour
    {
        [Header("Editable")]
        public GameObject player;
        public Tilemap tilemap;
        public GUIRenderer theGUIRenderer;

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
        private Dictionary<Coordinate, GameObject> sprites = new Dictionary<Coordinate, GameObject>();
        private float timeToAction;
        private AI ai;
        private bool rerenderRoom = true;
        private IEnumerator<int> enumerator = SGA.MainGenerator().GetEnumerator();

        void Start()
        {
            timeToAction = 1f;
            Config config = new Config();
            gameEngine = new GameEngine(config);
            ai = new AI();
        }

        private void Update()
        {
            // enumerator.MoveNext();
            // return;

            timeToAction -= Time.deltaTime;
            if (timeToAction < 0)
            {
                timeToAction = .3f;
                GameAction action = ai.NextMove(gameEngine);
                DoGameTick(action);
            }

            if (rerenderRoom)
            {
                tilemap.ClearAllTiles();

                gameEngine.checkMapTile(new Coordinate(0, 0));
                RenderRoom(gameEngine.map[new Coordinate(0, 0)], new Coordinate(0, 0));

                rerenderRoom = false;
            }

            theGUIRenderer.UpdateGUI(gameEngine.turnState, gameEngine.gameState, gameEngine.config);
        }

        private void DoGameTick(GameAction action)
        {
            var countOfRoomCleared = gameEngine.turnState.roomCleared.Count;
            var energy = gameEngine.turnState.energy;
            if (action == GameAction.Exit)
            {
                Debug.Log("Tokens: " + gameEngine.turnState.tokens + ", Money: " + gameEngine.turnState.money);
            }

            gameEngine.Tick(action);

            if (countOfRoomCleared < gameEngine.turnState.roomCleared.Count)
            {
                sprites.Remove(gameEngine.turnState.position, out GameObject sprite);
                Destroy(sprite);
            }
            if (action == GameAction.Exit || gameEngine.turnState.lives == 0)
            {
                rerenderRoom = true;
                gameEngine.config.seed++;
                gameEngine.NewGame();
                foreach (KeyValuePair<Coordinate, GameObject> sprite in sprites)
                {
                    Destroy(sprite.Value);
                }
                sprites.Clear();
            }

            if (action == GameAction.GoUp || action == GameAction.GoLeft || action == GameAction.GoDown || action == GameAction.GoRight)
            {
                // render room
                var pos = gameEngine.turnState.position;
                gameEngine.checkMapTile(pos);
                RenderRoom(gameEngine.map[pos], pos);
                RenderContent(gameEngine.map[pos], pos, gameEngine.turnState);
            }

            player.transform.DOMove(new Vector3(gameEngine.turnState.position.x * 8, gameEngine.turnState.position.y * 8, 0), timeToAction);
        }

        void RenderContent(MapTile mapTile, Coordinate coordinate, TurnState turnState)
        {
            if (sprites.ContainsKey(coordinate) || turnState.roomCleared.Contains(coordinate))
            {
                return;
            }
            if (mapTile.roomContent == MapRoomContent.Enemy)
            {
                GameObject newObj = Instantiate(enemyPrefab, new Vector3(coordinate.x * 8, coordinate.y * 8, 0), Quaternion.identity);
                newObj.GetComponent<SpriteRenderer>().sprite = enemies[gameEngine.GetEnemyLevel(coordinate) - 1];
                sprites.Add(coordinate, newObj);
            }
            if (mapTile.roomContent == MapRoomContent.Treasure)
            {
                GameObject newObj = Instantiate(enemyPrefab, new Vector3(coordinate.x * 8, coordinate.y * 8, 0), Quaternion.identity);
                newObj.GetComponent<SpriteRenderer>().sprite = chestClose;
                sprites.Add(coordinate, newObj);
            }
            if (mapTile.roomContent == MapRoomContent.Trader)
            {
                GameObject newObj = Instantiate(enemyPrefab, new Vector3(coordinate.x * 8, coordinate.y * 8, 0), Quaternion.identity);
                newObj.GetComponent<SpriteRenderer>().sprite = trader;
                sprites.Add(coordinate, newObj);
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

            if (mapTile.roomContent == MapRoomContent.Empty)
            {
                for (int x = 1; x < 7; x++)
                {
                    for (int y = 1; y < 7; y++)
                    {
                        if (x != 3 && x != 4 && y != 3 && y != 4)
                            room[y, x] = 0;
                    }
                }
            }

            if (!mapTile.entries.up)
            {
                room[0, 3] = 0;
                room[0, 4] = 0;
                if (mapTile.roomContent == MapRoomContent.Empty)
                {
                    room[1, 3] = 0;
                    room[1, 4] = 0;
                    room[2, 3] = 0;
                    room[2, 4] = 0;
                }
            }

            if (!mapTile.entries.left)
            {
                room[3, 0] = 0;
                room[4, 0] = 0;
                if (mapTile.roomContent == MapRoomContent.Empty)
                {
                    room[3, 1] = 0;
                    room[4, 1] = 0;
                    room[3, 2] = 0;
                    room[4, 2] = 0;
                }
            }

            if (!mapTile.entries.down)
            {
                room[7, 3] = 0;
                room[7, 4] = 0;
                if (mapTile.roomContent == MapRoomContent.Empty)
                {
                    room[6, 3] = 0;
                    room[6, 4] = 0;
                    room[5, 3] = 0;
                    room[5, 4] = 0;
                }
            }

            if (!mapTile.entries.right)
            {
                room[3, 7] = 0;
                room[4, 7] = 0;
                if (mapTile.roomContent == MapRoomContent.Empty)
                {
                    room[3, 6] = 0;
                    room[4, 6] = 0;
                    room[3, 5] = 0;
                    room[4, 5] = 0;
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
