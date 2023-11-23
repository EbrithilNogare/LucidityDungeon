using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    class ShoppingHallConnector : MonoBehaviour
    {
        [Header("Editable")]
        public ShoppingActionsRenderer shoppingActionsRenderer;
        public GameObject player;
        public Camera mainCamera;

        [Space(100)]

        [Header("From prefab")]
        public float duration;

        void Start()
        {

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
            var coordinate = new Coordinate((int)Math.Round(worldMousePosition.x), (int)Math.Round(worldMousePosition.y));

            player.transform.DOMove(new Vector3(coordinate.x, coordinate.y, player.transform.position.z), duration).SetSpeedBased(true); ;
        }
    }
}