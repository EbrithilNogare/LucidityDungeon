using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    class ShoppingActionsRenderer : MonoBehaviour
    {
        [Header("Public")]
        public ShoppingHallConnector shoppingHallConnector;

        [Space(100)]

        [Header("Private")]
        public GameObject initSpells;
        public GameObject spellStrength;
        public GameObject initPotions;
        public GameObject potionsStrength;
        public GameObject energyUpgrade;
        public GameObject enemyLevel;
        public GameObject enemyCount;
        public GameObject exit;

        private bool nearEnergyTrader = false;
        private bool nearPotionTrader = false;
        private bool nearSpellTrader = false;
        private bool nearEnemyTrader = false;
        private bool nearBed = false;

        void Start()
        {
            RenderShoppingActions();
        }

        public void OnClickInitSpells()
        {
            shoppingHallConnector.OnBuyInShoppingHall(ShoppingHallAction.upgradeInitSpells);
            RenderShoppingActions();
        }
        public void OnClickSpellStrength()
        {
            shoppingHallConnector.OnBuyInShoppingHall(ShoppingHallAction.upgradeSpellLevel);
            RenderShoppingActions();
        }
        public void OnClickInitPotions()
        {
            shoppingHallConnector.OnBuyInShoppingHall(ShoppingHallAction.upgradeInitPotions);
            RenderShoppingActions();
        }
        public void OnClickPotionsStrength()
        {
            shoppingHallConnector.OnBuyInShoppingHall(ShoppingHallAction.upgradePotionLevel);
            RenderShoppingActions();
        }
        public void OnClickEnergyUpgrade()
        {
            shoppingHallConnector.OnBuyInShoppingHall(ShoppingHallAction.upgradeEnergyLevel);
            RenderShoppingActions();
        }
        public void OnClickEnemyLevel()
        {
            shoppingHallConnector.OnBuyInShoppingHall(ShoppingHallAction.upgradeEnemyLevel);
            RenderShoppingActions();
        }
        public void OnClickEnemyCount()
        {
            shoppingHallConnector.OnBuyInShoppingHall(ShoppingHallAction.upgradeEnemyAndTreasureProbability);
            RenderShoppingActions();
        }
        public void OnClickExit()
        {
            Store._instance.SavePrefs();
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }

        public void RenderShoppingActions()
        {
            var actions = shoppingHallConnector?.gameEngine?.GetValidActionsInShoppingHall();
            initSpells.SetActive(nearSpellTrader && actions.Contains(ShoppingHallAction.upgradeInitSpells));
            spellStrength.SetActive(nearSpellTrader && actions.Contains(ShoppingHallAction.upgradeSpellLevel));
            initPotions.SetActive(nearPotionTrader && actions.Contains(ShoppingHallAction.upgradeInitPotions));
            potionsStrength.SetActive(nearPotionTrader && actions.Contains(ShoppingHallAction.upgradePotionLevel));
            energyUpgrade.SetActive(nearEnergyTrader && actions.Contains(ShoppingHallAction.upgradeEnergyLevel));
            enemyLevel.SetActive(nearEnemyTrader && actions.Contains(ShoppingHallAction.upgradeEnemyLevel));
            enemyCount.SetActive(nearEnemyTrader && actions.Contains(ShoppingHallAction.upgradeEnemyAndTreasureProbability));
            exit.SetActive(nearBed);
        }

        public void SetEnergyTraderOn()
        {
            nearEnergyTrader = true;
            RenderShoppingActions();
        }
        public void SetEnergyTraderOff()
        {
            nearEnergyTrader = false;
            RenderShoppingActions();
        }
        public void SetPotionTraderOn()
        {
            nearPotionTrader = true;
            RenderShoppingActions();
        }
        public void SetPotionTraderOff()
        {
            nearPotionTrader = false;
            RenderShoppingActions();
        }
        public void SetSpellTraderOn()
        {
            nearSpellTrader = true;
            RenderShoppingActions();
        }
        public void SetSpellTraderOff()
        {
            nearSpellTrader = false;
            RenderShoppingActions();
        }
        public void SetEnemyTraderOn()
        {
            nearEnemyTrader = true;
            RenderShoppingActions();
        }
        public void SetEnemyTraderOff()
        {
            nearEnemyTrader = false;
            RenderShoppingActions();
        }
        public void SetBedOn()
        {
            nearBed = true;
            RenderShoppingActions();
        }
        public void SetBedOff()
        {
            nearBed = false;
            RenderShoppingActions();
        }
    }
}
