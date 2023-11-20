using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    class ActionsRenderer : MonoBehaviour
    {
        public GameObject Attack;
        public GameObject UsePotion;
        public GameObject UseSpell;
        public GameObject OpenChest;
        public GameObject BuyToken;
        public GameObject BuyPotion;
        public GameObject BuySpell;
        public GameObject Exit;

        void Start()
        {

        }

        void Update()
        {

        }

        public void RenderActions(List<GameAction> actions)
        {
            Attack.SetActive(actions.Contains(GameAction.Attack));
            UsePotion.SetActive(actions.Contains(GameAction.UsePotion));
            UseSpell.SetActive(actions.Contains(GameAction.UseSpell));
            OpenChest.SetActive(actions.Contains(GameAction.OpenChest));
            BuyToken.SetActive(actions.Contains(GameAction.BuyToken));
            BuyPotion.SetActive(actions.Contains(GameAction.BuyPotion));
            BuySpell.SetActive(actions.Contains(GameAction.BuySpell));
            Exit.SetActive(actions.Contains(GameAction.Exit));
        }
    }
}
