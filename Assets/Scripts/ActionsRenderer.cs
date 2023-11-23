using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    class ActionsRenderer : MonoBehaviour
    {
        [Header("Editable")]
        public GameEngineConnector gameEngineCOnnector;

        [Space(100)]

        [Header("From prefab")]
        public GameObject attack;
        public GameObject usePotion;
        public GameObject useSpell;
        public GameObject openChest;
        public GameObject buyToken;
        public GameObject buyPotion;
        public GameObject buySpell;
        public GameObject exit;
        public TextMeshProUGUI potionValue;

        public void Attack() { gameEngineCOnnector.AddActionToQueue(GameAction.Attack, true); }
        public void UsePotion() { gameEngineCOnnector.AddActionToQueue(GameAction.UsePotion, true); }
        public void UseSpell() { gameEngineCOnnector.AddActionToQueue(GameAction.UseSpell, true); }
        public void OpenChest() { gameEngineCOnnector.AddActionToQueue(GameAction.OpenChest, true); }
        public void BuyToken() { gameEngineCOnnector.AddActionToQueue(GameAction.BuyToken, true); }
        public void BuyPotion() { gameEngineCOnnector.AddActionToQueue(GameAction.BuyPotion, true); }
        public void BuySpell() { gameEngineCOnnector.AddActionToQueue(GameAction.BuySpell, true); }
        public void Exit() { gameEngineCOnnector.AddActionToQueue(GameAction.Exit, true); }

        public void RenderActions(List<GameAction> actions, TurnState turnState, Config config)
        {
            attack.SetActive(actions.Contains(GameAction.Attack));
            usePotion.SetActive(actions.Contains(GameAction.UsePotion));
            useSpell.SetActive(actions.Contains(GameAction.UseSpell));
            openChest.SetActive(actions.Contains(GameAction.OpenChest));
            buyToken.SetActive(actions.Contains(GameAction.BuyToken));
            buyPotion.SetActive(actions.Contains(GameAction.BuyPotion));
            buySpell.SetActive(actions.Contains(GameAction.BuySpell));
            exit.SetActive(actions.Contains(GameAction.Exit));
        }
    }
}
