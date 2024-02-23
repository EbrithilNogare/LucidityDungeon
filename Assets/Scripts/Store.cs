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
            Dreamwalker,
            Keymaster,
            TokenHoarder,
            Energized,
            ArcaneScholar,
            PotionConnoisseur,
            HardcoreWarrior,
            LuckyJoe,
        }

        private Dictionary<string, string> AchievementsText = new Dictionary<string, string> {
            { "Dreamwalker", "Successfuly complete dream run." },
            { "Keymaster", "Open 20 chests in single dream run." },
            { "Token Hoarder", "Accumulate 500 tokens." },
            { "Energized", "Purchase full energy upgrade." },
            { "Arcane Scholar", "Collect all init spells and upgrades." },
            { "Potion Connoisseur", "Acquire all potions and upgrades." },
            { "Hardcore Warrior", "Call as many strongest beasts into dungeon as possible." },
            { "End?", "Get 1000 tokens from a single game." },
        };

        // Store._instance.HandleAchievementProgress(Store.AchievementProgressType.ArcaneScholar);

        public void HandleAchievementProgress(AchievementProgressType achievementProgressType)
        {
            int achievementProgressBefore = achievementProgress;

            // save data
            switch (achievementProgressType)
            {
                case AchievementProgressType.Dreamwalker: achievementProgress |= (1 << 0); break;
                case AchievementProgressType.Keymaster: achievementProgress |= (1 << 1); break;
                case AchievementProgressType.TokenHoarder: achievementProgress |= (1 << 2); break;
                case AchievementProgressType.Energized: achievementProgress |= (1 << 3); break;
                case AchievementProgressType.ArcaneScholar: achievementProgress |= (1 << 4); break;
                case AchievementProgressType.PotionConnoisseur: achievementProgress |= (1 << 5); break;
                case AchievementProgressType.HardcoreWarrior: achievementProgress |= (1 << 6); break;
                case AchievementProgressType.LuckyJoe: achievementProgress |= (1 << 7); break;
            }

            if (achievementProgressBefore == achievementProgress)
            {
                return;
            }

            SavePrefs();

            // show badge
            card.SetActive(true);
            switch (achievementProgressType)
            {
                case AchievementProgressType.Dreamwalker: title.SetText("Dreamwalker"); label.SetText(AchievementsText["Dreamwalker"]); break;
                //case AchievementProgressType.Keymaster: title.SetText("Keymaster"); label.SetText(AchievementsText["Keymaster"]); break;
                //case AchievementProgressType.TokenHoarder: title.SetText("Token Hoarder"); label.SetText(AchievementsText["Token Hoarder"]); break;
                //case AchievementProgressType.Energized: title.SetText("Energized"); label.SetText(AchievementsText["Energized"]); break;
                //case AchievementProgressType.ArcaneScholar: title.SetText("Arcane Scholar"); label.SetText(AchievementsText["Arcane Scholar"]); break;
                //case AchievementProgressType.PotionConnoisseur: title.SetText("Potion Connoisseur"); label.SetText(AchievementsText["Potion Connoisseur"]); break;
                case AchievementProgressType.HardcoreWarrior: title.SetText("Hardcore Warrior"); label.SetText(AchievementsText["Hardcore Warrior"]); break;
                    //case AchievementProgressType.LuckyJoe: title.SetText("End?"); label.SetText(AchievementsText["End?"]); break;
            }
            card.transform.localScale = new Vector3(0, 0, 1);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(card.transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.InOutExpo));
            sequence.Append(card.transform.DOScale(new Vector3(1.05f, 1.05f, 1f), 4f).SetEase(Ease.Linear));
            sequence.Append(card.transform.DOScale(new Vector3(0, 0, 1), .5f).SetEase(Ease.InOutExpo));
            sequence.OnComplete(() =>
            {
                card.SetActive(false);
            });
        }
    }
}
