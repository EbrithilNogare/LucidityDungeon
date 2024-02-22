using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    class GameEngineConnector : MonoBehaviour
    {
        [Header("Editable")]
        public GameObject player;
        public Animator swordHitAnimator;
        public Animator spellHitAnimator;
        public Animator clawsHitAnimator;
        public ParticleSystem usePotionParticles;
        public ParticleSystem OpenChestParticles;
        public AudioSource swordSound;
        public AudioSource spellSound;
        public AudioSource healSound;
        public AudioSource buySound;
        public AudioSource openChestSound;
        public Tilemap tilemap;
        public GUIRenderer theGUIRenderer;
        public ActionsRenderer actionsRenderer;
        public Camera mainCamera;
        public GameObject shadowCasterPrefab;

        [Space(100)]

        [Header("From prefab")]
        public ConstDictionary constDictionary;

        public GameObject enemyPrefab;
        public int minCameraSize;
        public int maxCameraSize;
        public bool aiAutoplay;
        public float timeToAction;

        private GameEngine gameEngine;
        private Dictionary<Coordinate, GameObject> sprites;
        private float nextActionTimeout;
        private AI ai;
        private bool renderNewGame;
        private IEnumerator<int> enumerator;
        private HashSet<Coordinate> alreadyRenderedRooms;
        private bool firstTouch = true;
        private List<GameAction> actionsInQueue;


        void Start()
        {
            nextActionTimeout = timeToAction;
            renderNewGame = true;
            Config config = new Config();
            gameEngine = new GameEngine(config, Store._instance.gameState);
            ai = new AI();
            enumerator = SGA.MainGenerator().GetEnumerator();
            actionsInQueue = new List<GameAction>();
            alreadyRenderedRooms = new HashSet<Coordinate>();
            sprites = new Dictionary<Coordinate, GameObject>();
            SetAudioVolume();
        }

        private void SetAudioVolume()
        {
            float soundVolume = Store._instance.sounds == 0 ? 0 : Mathf.Pow(10f, Mathf.Lerp(-40f, 0f, Store._instance.sounds / 5f) / 20f);
            swordSound.volume = soundVolume;
            spellSound.volume = soundVolume;
            healSound.volume = soundVolume;
            buySound.volume = soundVolume;
            openChestSound.volume = soundVolume;
        }

        private void Update()
        {
            /*/ // switch for SGA computation
            enumerator.MoveNext();
            return;
            /**/

            if (aiAutoplay && actionsInQueue.Count == 0)
            {
                actionsInQueue.Add(ai.NextMove(gameEngine));
            }

            nextActionTimeout -= Time.deltaTime;
            if (actionsInQueue.Count > 0 && nextActionTimeout < 0)
            {
                nextActionTimeout = timeToAction;
                var action = actionsInQueue[0];
                actionsInQueue.RemoveAt(0);
                DoGameTick(action);
                theGUIRenderer.UpdateGUI(gameEngine);
                actionsRenderer.RenderActions(gameEngine.GetValidActions(), gameEngine.turnState, gameEngine.gameState, gameEngine.config);
            }

            if (renderNewGame)
            {
                // tilemap
                alreadyRenderedRooms.Clear();
                tilemap.ClearAllTiles();
                gameEngine.checkMapTile(new Coordinate(0, 0));
                RenderRoom(gameEngine.map[new Coordinate(0, 0)], new Coordinate(0, 0));
                // sprites
                foreach (KeyValuePair<Coordinate, GameObject> sprite in sprites)
                {
                    Destroy(sprite.Value);
                }
                sprites.Clear();

                theGUIRenderer.UpdateGUI(gameEngine);
                actionsRenderer.RenderActions(gameEngine.GetValidActions(), gameEngine.turnState, gameEngine.gameState, gameEngine.config);

                renderNewGame = false;
            }

            CheckPinch();
        }

        public void AddActionToQueue(GameAction gameAction, bool removeOtherActions = false)
        {
            AddActionToQueue(new List<GameAction>(1) { gameAction }, removeOtherActions);
        }

        public void AddActionToQueue(List<GameAction> gameActions, bool removeOtherActions = false)
        {
            if (removeOtherActions)
            {
                actionsInQueue.Clear();
            }
            actionsInQueue.AddRange(gameActions);
        }

        public void Click(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            if (EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject(-1)) return;
            Vector2 position = Mouse.current.position.ReadValue();
            ResolveClick(position);
        }

        public void Touch(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId)) return;
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            ResolveClick(touchPosition);
        }

        public void ResolveClick(Vector2 position)
        {
            float cameraSize = mainCamera.orthographicSize;
            Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(new Vector3(position.x, position.y, cameraSize));
            var coordinate = new Coordinate((int)Math.Round(worldMousePosition.x / 8), (int)Math.Round(worldMousePosition.y / 8));
            PerformNavigation(coordinate);
        }

        private void PerformNavigation(Coordinate coordinate)
        {
            if (gameEngine.isEnemyInMyRoom(gameEngine.turnState))
            {
                return;
            }

            if (gameEngine.map.ContainsKey(coordinate))
            {
                AddActionToQueue(gameEngine.GetPathFromSourceToGoal(gameEngine.turnState.position, coordinate), true);
                return;
            }

            var neighborCoordinates = new Coordinate(coordinate.x + 1, coordinate.y);
            if (gameEngine.map.ContainsKey(neighborCoordinates) && gameEngine.map[neighborCoordinates].entries.left)
            {
                AddActionToQueue(gameEngine.GetPathFromSourceToGoal(gameEngine.turnState.position, coordinate), true);
                return;
            }

            neighborCoordinates = new Coordinate(coordinate.x - 1, coordinate.y);
            if (gameEngine.map.ContainsKey(neighborCoordinates) && gameEngine.map[neighborCoordinates].entries.right)
            {
                AddActionToQueue(gameEngine.GetPathFromSourceToGoal(gameEngine.turnState.position, coordinate), true);
                return;
            }

            neighborCoordinates = new Coordinate(coordinate.x, coordinate.y + 1);
            if (gameEngine.map.ContainsKey(neighborCoordinates) && gameEngine.map[neighborCoordinates].entries.down)
            {
                AddActionToQueue(gameEngine.GetPathFromSourceToGoal(gameEngine.turnState.position, coordinate), true);
                return;
            }

            neighborCoordinates = new Coordinate(coordinate.x, coordinate.y - 1);
            if (gameEngine.map.ContainsKey(neighborCoordinates) && gameEngine.map[neighborCoordinates].entries.up)
            {
                AddActionToQueue(gameEngine.GetPathFromSourceToGoal(gameEngine.turnState.position, coordinate), true);
                return;
            }
        }

        public void Zoom(InputAction.CallbackContext context)
        {
            resolveZoom(context.ReadValue<float>() / -120);
        }

        private void CheckPinch()
        {
            if (Input.touchCount == 2)
            {
                if (firstTouch)
                {
                    firstTouch = false;
                    return;
                }

                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPosition = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPosition = touchOne.position - touchOne.deltaPosition;

                float previousMagnitude = (touchZeroPrevPosition - touchOnePrevPosition).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - previousMagnitude;

                resolveZoom(difference * .01f);
            }
            else
            {
                firstTouch = true;
            }
        }

        public void resolveZoom(float difference)
        {
            mainCamera.orthographicSize = Math.Min(Math.Max(mainCamera.orthographicSize + difference, minCameraSize), maxCameraSize);
        }

        private void DoGameTick(GameAction action)
        {
            var countOfRoomCleared = gameEngine.turnState.roomCleared.Count;

            if (action == GameAction.Attack)
            {
                swordHitAnimator.Play("sword hit", 0, 0);
                swordSound.Play();
            }

            if (action == GameAction.UseSpell)
            {
                spellHitAnimator.Play("spell hit", 0, 0);
                spellSound.Play();
            }

            if (action == GameAction.UsePotion)
            {
                usePotionParticles.Play();
                healSound.Play();
            }

            if (action == GameAction.BuyPotion || action == GameAction.BuySpell || action == GameAction.BuyToken)
            {
                buySound.Play();
            }

            if (action == GameAction.OpenChest)
            {
                OpenChestParticles.Play();
                openChestSound.Play();
            }

            gameEngine.Tick(action);


            if (action == GameAction.Attack && gameEngine.turnState.enemyHp > 0)
            {
                clawsHitAnimator.Play("claws hit", 0, 0);
            }

            if (countOfRoomCleared < gameEngine.turnState.roomCleared.Count)
            {
                sprites.Remove(gameEngine.turnState.position, out GameObject sprite);
                Destroy(sprite);
            }

            if (action == GameAction.Exit)
            {
                Store._instance.gameState = gameEngine.gameState;
                Store._instance.SavePrefs();
                SceneManager.LoadScene("Shopping hall", LoadSceneMode.Single);
                return;
            }

            if (gameEngine.turnState.lives == 0)
            {
                // todo die
                renderNewGame = true;
                gameEngine.config.seed++;
                gameEngine.NewGame();
            }

            if (action == GameAction.GoUp || action == GameAction.GoLeft || action == GameAction.GoDown || action == GameAction.GoRight)
            {
                var pos = gameEngine.turnState.position;
                gameEngine.checkMapTile(pos);
                RenderRoom(gameEngine.map[pos], pos);
                RenderContent(gameEngine.map[pos], pos, gameEngine.turnState);
            }

            player.transform.DOMove(new Vector3(gameEngine.turnState.position.x * 8, gameEngine.turnState.position.y * 8, player.transform.position.z), timeToAction);
        }

        void RenderContent(MapTile mapTile, Coordinate coordinate, TurnState turnState)
        {
            if (alreadyRenderedRooms.Contains(coordinate))
            {
                return;
            }
            alreadyRenderedRooms.Add(coordinate);
            if (mapTile.roomContent == MapRoomContent.Enemy)
            {
                GameObject newObj = Instantiate(enemyPrefab, new Vector3(coordinate.x * 8 + 1.5f, coordinate.y * 8, 0), Quaternion.identity);
                newObj.SetActive(true);
                newObj.GetComponent<SpriteRenderer>().sprite = constDictionary.enemies[gameEngine.GetEnemyLevel(coordinate) - 1];
                sprites.Add(coordinate, newObj);
            }
            if (mapTile.roomContent == MapRoomContent.Treasure)
            {
                GameObject newObj = Instantiate(enemyPrefab, new Vector3(coordinate.x * 8 + 1.5f, coordinate.y * 8, 0), Quaternion.identity);
                newObj.SetActive(true);
                newObj.GetComponent<SpriteRenderer>().sprite = constDictionary.chestClose;
                sprites.Add(coordinate, newObj);
            }
            if (mapTile.roomContent == MapRoomContent.Trader)
            {
                GameObject newObj = Instantiate(enemyPrefab, new Vector3(coordinate.x * 8 + 1.5f, coordinate.y * 8, 0), Quaternion.identity);
                newObj.SetActive(true);
                newObj.GetComponent<SpriteRenderer>().sprite = constDictionary.trader;
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
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), constDictionary.floorTiles[randomIndex % constDictionary.floorTiles.Length]);
                        continue;
                    }

                    if (7 - y + 1 <= 7 && room[7 - y + 1, x] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), constDictionary.wallTiles[randomIndex % constDictionary.wallTiles.Length]);
                        continue;
                    }

                    if (x + 1 <= 7 && room[7 - y, x + 1] == 1 && 7 - y - 1 >= 0 && room[7 - y - 1, x] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), constDictionary.edgeUpRightTiles[randomIndex % constDictionary.edgeUpRightTiles.Length]);
                        continue;
                    }

                    if (x - 1 >= 0 && room[7 - y, x - 1] == 1 && 7 - y - 1 >= 0 && room[7 - y - 1, x] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), constDictionary.edgeUpLeftTiles[randomIndex % constDictionary.edgeUpLeftTiles.Length]);
                        continue;
                    }

                    if (7 - y - 1 >= 0 && room[7 - y - 1, x] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), constDictionary.edgeUpTiles[randomIndex % constDictionary.edgeUpTiles.Length]);
                        continue;
                    }

                    if ((x - 1 >= 0 && room[7 - y, x - 1] == 1) || (x - 1 >= 0 && 7 - y + 1 <= 7 && room[7 - y + 1, x - 1] == 1))
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), constDictionary.edgeLeftTiles[randomIndex % constDictionary.edgeLeftTiles.Length]);
                        continue;
                    }

                    if ((x + 1 <= 7 && room[7 - y, x + 1] == 1) || (x + 1 <= 7 && 7 - y + 1 <= 7 && room[7 - y + 1, x + 1] == 1))
                    {
                        tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), constDictionary.edgeRightTiles[randomIndex % constDictionary.edgeRightTiles.Length]);
                        continue;
                    }

                    tilemap.SetTile(new Vector3Int(coordinate.x * 8 + x, coordinate.y * 8 + y, 0), constDictionary.nothingTiles[randomIndex % constDictionary.nothingTiles.Length]);
                }
            }

            if (mapTile.entries.up && tilemap.GetTile(new Vector3Int(coordinate.x * 8 + 3, coordinate.y * 8 + 8, 0)) == null)
            {
                tilemap.SetTile(new Vector3Int(coordinate.x * 8 + 3, coordinate.y * 8 + 8, 0), constDictionary.tileToEmptyRoom[0]);
                tilemap.SetTile(new Vector3Int(coordinate.x * 8 + 4, coordinate.y * 8 + 8, 0), constDictionary.tileToEmptyRoom[0]);
            }
            if (mapTile.entries.left && tilemap.GetTile(new Vector3Int(coordinate.x * 8 - 1, coordinate.y * 8 + 4, 0)) == null)
            {
                tilemap.SetTile(new Vector3Int(coordinate.x * 8 - 1, coordinate.y * 8 + 4, 0), constDictionary.tileToEmptyRoom[1]);
                tilemap.SetTile(new Vector3Int(coordinate.x * 8 - 1, coordinate.y * 8 + 3, 0), constDictionary.tileToEmptyRoom[1]);
            }
            if (mapTile.entries.down && tilemap.GetTile(new Vector3Int(coordinate.x * 8 + 3, coordinate.y * 8 - 1, 0)) == null)
            {
                tilemap.SetTile(new Vector3Int(coordinate.x * 8 + 3, coordinate.y * 8 - 1, 0), constDictionary.tileToEmptyRoom[2]);
                tilemap.SetTile(new Vector3Int(coordinate.x * 8 + 4, coordinate.y * 8 - 1, 0), constDictionary.tileToEmptyRoom[2]);
            }
            if (mapTile.entries.right && tilemap.GetTile(new Vector3Int(coordinate.x * 8 + 8, coordinate.y * 8 + 4, 0)) == null)
            {
                tilemap.SetTile(new Vector3Int(coordinate.x * 8 + 8, coordinate.y * 8 + 4, 0), constDictionary.tileToEmptyRoom[3]);
                tilemap.SetTile(new Vector3Int(coordinate.x * 8 + 8, coordinate.y * 8 + 3, 0), constDictionary.tileToEmptyRoom[3]);
            }
        }

    }
}
