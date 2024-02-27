using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    class Store : MonoBehaviour
    {
        public static Store _instance;
        public GameObject card;
        public TextMeshProUGUI title;
        public TextMeshProUGUI label;

        private int _volume;
        public int volume
        {
            get { return _volume; }
            set
            {
                float mappedValue = value == 0 ? 0 : Mathf.Pow(10f, Mathf.Lerp(-40f, 0f, value / 5f) / 20f);
                transform.GetComponent<AudioSource>().volume = mappedValue;
                _volume = value;
            }
        }
        public int sounds;
        public GameState gameState;
        public int achievementProgress;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            LoadPrefs();
        }

        public void SavePrefs()
        {
            // Settings
            PlayerPrefs.SetInt("volume", volume);
            PlayerPrefs.SetInt("sounds", sounds);

            // Game
            PlayerPrefs.SetInt("upgradeEnemyAndTreasureProbability", gameState.upgradeEnemyAndTreasureProbability);
            PlayerPrefs.SetInt("upgradeEnemyLevel", gameState.upgradeEnemyLevel);
            PlayerPrefs.SetInt("upgradePotionLevel", gameState.upgradePotionLevel);
            PlayerPrefs.SetInt("upgradeInitPotions", gameState.upgradeInitPotions);
            PlayerPrefs.SetInt("upgradeSpellLevel", gameState.upgradeSpellLevel);
            PlayerPrefs.SetInt("upgradeInitSpells", gameState.upgradeInitSpells);
            PlayerPrefs.SetInt("upgradeEnergyLevel", gameState.upgradeEnergyLevel);
            PlayerPrefs.SetInt("lastRunTokens", gameState.lastRunTokens);
            PlayerPrefs.SetInt("achievementProgress", achievementProgress);

            PlayerPrefs.Save();
        }

        public void LoadPrefs()
        {
            // Settings
            volume = PlayerPrefs.GetInt("volume", 3);
            sounds = PlayerPrefs.GetInt("sounds", 3);

            // Game
            int upgradeEnemyAndTreasureProbability = PlayerPrefs.GetInt("upgradeEnemyAndTreasureProbability", 0);
            int upgradeEnemyLevel = PlayerPrefs.GetInt("upgradeEnemyLevel", 0);
            int upgradePotionLevel = PlayerPrefs.GetInt("upgradePotionLevel", 0);
            int upgradeInitPotions = PlayerPrefs.GetInt("upgradeInitPotions", 0);
            int upgradeSpellLevel = PlayerPrefs.GetInt("upgradeSpellLevel", 0);
            int upgradeInitSpells = PlayerPrefs.GetInt("upgradeInitSpells", 0);
            int upgradeEnergyLevel = PlayerPrefs.GetInt("upgradeEnergyLevel", 0);
            int lastRunTokens = PlayerPrefs.GetInt("lastRunTokens", 0);
            achievementProgress = PlayerPrefs.GetInt("achievementProgress", 0);

            gameState = new GameState(
                upgradeEnemyAndTreasureProbability,
                upgradeEnemyLevel,
                upgradePotionLevel,
                upgradeInitPotions,
                upgradeSpellLevel,
                upgradeInitSpells,
                upgradeEnergyLevel,
                lastRunTokens
            );
        }

        public enum AchievementProgressType
        {
            CompleteDungeon,
            OpenManyChests,
            GetManyCoins,
            BuyAllEnergyUpgrades,
            BuyAllSpellUpgrades,
            BuyAllPotionUpgrades,
            BuyAllEnemyUpgrades,
            LuckyJoe,
        }

        private Dictionary<AchievementProgressType, (string title, string description)> AchievementsText = new()
        {
            { AchievementProgressType.CompleteDungeon, ("Dreamwalker", "Successfully complete dream run.") },
            { AchievementProgressType.OpenManyChests, ("Keymaster", "Open 10 chests in single dream run.") },
            { AchievementProgressType.GetManyCoins, ("Token Hoarder", "Accumulate 500 coins.") },
            { AchievementProgressType.BuyAllEnergyUpgrades, ("Energized", "Purchase full energy upgrade.") },
            { AchievementProgressType.BuyAllSpellUpgrades, ("Arcane Scholar", "Collect all init spells and upgrades.") },
            { AchievementProgressType.BuyAllPotionUpgrades, ("Potion Connoisseur", "Acquire all potions and upgrades.") },
            { AchievementProgressType.BuyAllEnemyUpgrades, ("Hardcore Warrior", "Call as many strongest beasts into dungeon as possible.") },
            { AchievementProgressType.LuckyJoe, ("Lucky Joe", "Survive with only 1 HP") },
        };

        // call like this:
        // Store._instance.HandleAchievementProgress(Store.AchievementProgressType.CompleteDungeon);
        public void HandleAchievementProgress(AchievementProgressType achievementProgressType)
        {
            int achievementProgressBefore = achievementProgress;

            // save binary data
            switch (achievementProgressType)
            {
                case AchievementProgressType.CompleteDungeon: achievementProgress |= (1 << 0); break;
                case AchievementProgressType.OpenManyChests: achievementProgress |= (1 << 1); break;
                case AchievementProgressType.GetManyCoins: achievementProgress |= (1 << 2); break;
                case AchievementProgressType.BuyAllEnergyUpgrades: achievementProgress |= (1 << 3); break;
                case AchievementProgressType.BuyAllSpellUpgrades: achievementProgress |= (1 << 4); break;
                case AchievementProgressType.BuyAllPotionUpgrades: achievementProgress |= (1 << 5); break;
                case AchievementProgressType.BuyAllEnemyUpgrades: achievementProgress |= (1 << 6); break;
                case AchievementProgressType.LuckyJoe: achievementProgress |= (1 << 7); break;
            }

            if (achievementProgressBefore == achievementProgress)
            {
                return;
            }

            SavePrefs();

            // show badge
            card.SetActive(true);
            title.SetText(AchievementsText[achievementProgressType].title);
            label.SetText(AchievementsText[achievementProgressType].description);

            card.transform.localScale = new Vector3(0, 0, 1);
            var sequence = DOTween.Sequence();
            sequence.Append(card.transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.InOutExpo));
            sequence.Append(card.transform.DOScale(new Vector3(1.05f, 1.05f, 1f), 4f).SetEase(Ease.Linear));
            sequence.Append(card.transform.DOScale(new Vector3(0, 0, 1), .5f).SetEase(Ease.InOutExpo));
            sequence.OnComplete(() => { card.SetActive(false); });
        }
    }
}
