using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    class ShoppingHallConnector : MonoBehaviour
    {
        [Header("Editable")]
        public ShoppingActionsRenderer shoppingActionsRenderer;
        public ShoppingHallGUIRenderer shoppingHallGUIRenderer;
        public GameObject player;
        public Camera mainCamera;
        public GameEngine gameEngine;
        public TilemapAStar tilemapAStar;

        [Space(100)]

        [Header("From prefab")]
        public float duration;

        private Sequence sequence;

        void Start()
        {
            Config config = new Config();
            gameEngine = new GameEngine(config);
            gameEngine.gameState = Store._instance.gameState;
            sequence = DOTween.Sequence();

            if (gameEngine.gameState.lastRunTokens > 0)
            {
                Store._instance.HandleAchievementProgress(Store.AchievementProgressType.Dreamwalker);
            }
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
            var touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            ResolveClick(touchPosition);
        }

        public void ResolveClick(Vector2 position)
        {
            float cameraSize = mainCamera.orthographicSize;
            Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(new Vector3(position.x, position.y, cameraSize));

            DOTween.Kill(player.transform, complete: false);

            Vector2Int start = new Vector2Int((int)Mathf.Round(player.transform.position.x - .5f), (int)Mathf.Round(player.transform.position.y - 1));
            Vector2Int end = new Vector2Int((int)Mathf.Floor(worldMousePosition.x), (int)Mathf.Floor(worldMousePosition.y));
            var path = tilemapAStar.FindPath(start, end);
            Vector2Int currentPosition = start;

            sequence.Kill(complete: false);
            sequence = DOTween.Sequence();

            foreach (var direction in path)
            {
                switch (direction)
                {
                    case TilemapAStar.Direction.Up: currentPosition += Vector2Int.up; break;
                    case TilemapAStar.Direction.Down: currentPosition += Vector2Int.down; break;
                    case TilemapAStar.Direction.Left: currentPosition += Vector2Int.left; break;
                    case TilemapAStar.Direction.Right: currentPosition += Vector2Int.right; break;
                }

                sequence.Append(player.transform.DOMove(new Vector3(currentPosition.x + .5f, currentPosition.y + 1, player.transform.position.z), duration).SetEase(Ease.Linear));
            }

            tilemapAStar.PrintDebug(path, new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y));
        }

        public void OnBuyInShoppingHall(ShoppingHallAction action)
        {
            gameEngine.BuyInShoppingHall(action);
            Store._instance.gameState = gameEngine.gameState;
            Store._instance.SavePrefs();
            shoppingHallGUIRenderer.RenderGUI();

            Config config = new Config();
            if ((action == ShoppingHallAction.upgradeEnemyLevel || action == ShoppingHallAction.upgradeEnemyAndTreasureProbability) && gameEngine.gameState.upgradeEnemyLevel == config.enemyLevelPrices.Length - 1 && gameEngine.gameState.upgradeEnemyAndTreasureProbability == config.enemyProbabilityPrices.Length - 1)
            {
                Store._instance.HandleAchievementProgress(Store.AchievementProgressType.HardcoreWarrior);
            }
        }
    }
}