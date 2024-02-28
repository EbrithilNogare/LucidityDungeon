using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        public GameObject toDungeon;
        public GameObject exit;

        public TextMeshProUGUI initSpellsPrice;
        public TextMeshProUGUI spellStrengthPrice;
        public TextMeshProUGUI initPotionsPrice;
        public TextMeshProUGUI potionsStrengthPrice;
        public TextMeshProUGUI energyUpgradePrice;
        public TextMeshProUGUI enemyLevelPrice;
        public TextMeshProUGUI enemyCountPrice;

        private bool nearEnergyTrader = false;
        private bool nearPotionTrader = false;
        private bool nearSpellTrader = false;
        private bool nearEnemyTrader = false;
        private bool nearBed = false;

        private Config config;

        void Start()
        {
            config = new Config();
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
        public void OnClickToDungeon()
        {
            Store._instance.SavePrefs();
            DOTween.KillAll(false);
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
        public void OnClickExit()
        {
            Store._instance.SavePrefs();
            DOTween.KillAll(false);
            SceneManager.LoadScene("Main menu", LoadSceneMode.Single);
        }

        public void RenderShoppingActions()
        {
            var gameState = shoppingHallConnector?.gameEngine?.gameState ?? new GameState();
            initSpellsPrice.SetText(config.upgradeInitSpellsPrices[(gameState.upgradeInitSpells + 1) % config.upgradeInitSpellsPrices.Length].ToString());
            spellStrengthPrice.SetText(config.upgradeSpellPrices[(gameState.upgradeSpellLevel + 1) % config.upgradeSpellPrices.Length].ToString());
            initPotionsPrice.SetText(config.upgradeInitPotionsPrices[(gameState.upgradeInitPotions + 1) % config.upgradeInitPotionsPrices.Length].ToString());
            potionsStrengthPrice.SetText(config.upgradePotionPrices[(gameState.upgradePotionLevel + 1) % config.upgradePotionPrices.Length].ToString());
            energyUpgradePrice.SetText(config.energyPrices[(gameState.upgradeEnergyLevel + 1) % config.energyPrices.Length].ToString());
            enemyLevelPrice.SetText(config.enemyLevelPrices[(gameState.upgradeEnemyLevel + 1) % config.enemyLevelPrices.Length].ToString());
            enemyCountPrice.SetText(config.enemyProbabilityPrices[(gameState.upgradeEnemyAndTreasureProbability + 1) % config.enemyProbabilityPrices.Length].ToString());


            var actions = shoppingHallConnector?.gameEngine?.GetValidActionsInShoppingHall();
            initSpells.SetActive(nearSpellTrader && initSpellsPrice.text != "0");
            if (nearSpellTrader) initSpells.GetComponent<Button>().interactable = actions.Contains(ShoppingHallAction.upgradeInitSpells);
            spellStrength.SetActive(nearSpellTrader && spellStrengthPrice.text != "0");
            if (nearSpellTrader) spellStrength.GetComponent<Button>().interactable = actions.Contains(ShoppingHallAction.upgradeSpellLevel);

            initPotions.SetActive(nearPotionTrader && initPotionsPrice.text != "0");
            if (nearPotionTrader) initPotions.GetComponent<Button>().interactable = actions.Contains(ShoppingHallAction.upgradeInitPotions);
            potionsStrength.SetActive(nearPotionTrader && potionsStrengthPrice.text != "0");
            if (nearPotionTrader) potionsStrength.GetComponent<Button>().interactable = actions.Contains(ShoppingHallAction.upgradePotionLevel);

            energyUpgrade.SetActive(nearEnergyTrader && energyUpgradePrice.text != "0");
            if (nearEnergyTrader) energyUpgrade.GetComponent<Button>().interactable = actions.Contains(ShoppingHallAction.upgradeEnergyLevel);

            enemyLevel.SetActive(nearEnemyTrader && enemyLevelPrice.text != "0");
            if (nearEnemyTrader) enemyLevel.GetComponent<Button>().interactable = actions.Contains(ShoppingHallAction.upgradeEnemyLevel);
            enemyCount.SetActive(nearEnemyTrader && enemyCountPrice.text != "0");
            if (nearEnemyTrader) enemyCount.GetComponent<Button>().interactable = actions.Contains(ShoppingHallAction.upgradeEnemyAndTreasureProbability);

            toDungeon.SetActive(nearBed);
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
