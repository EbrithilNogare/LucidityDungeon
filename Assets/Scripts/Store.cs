using UnityEngine;

namespace Assets.Scripts
{

    class Store : MonoBehaviour
    {
        public static Store instance;

        public int volume;
        public int sounds;
        public GameState gameState;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
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
            PlayerPrefs.SetInt("permanentTokens", gameState.permanentTokens);

            PlayerPrefs.Save();
        }

        public void LoadPrefs()
        {
            // Settings
            volume = PlayerPrefs.GetInt("volume", 2);
            sounds = PlayerPrefs.GetInt("sounds", 2);

            // Game
            int upgradeEnemyAndTreasureProbability = PlayerPrefs.GetInt("upgradeEnemyAndTreasureProbability", 0);
            int upgradeEnemyLevel = PlayerPrefs.GetInt("upgradeEnemyLevel", 0);
            int upgradePotionLevel = PlayerPrefs.GetInt("upgradePotionLevel", 0);
            int upgradeInitPotions = PlayerPrefs.GetInt("upgradeInitPotions", 0);
            int upgradeSpellLevel = PlayerPrefs.GetInt("upgradeSpellLevel", 0);
            int upgradeInitSpells = PlayerPrefs.GetInt("upgradeInitSpells", 0);
            int upgradeEnergyLevel = PlayerPrefs.GetInt("upgradeEnergyLevel", 0);
            int permanentTokens = PlayerPrefs.GetInt("permanentTokens", 0);

            gameState = new GameState(
                upgradeEnemyAndTreasureProbability,
                upgradeEnemyLevel,
                upgradePotionLevel,
                upgradeInitPotions,
                upgradeSpellLevel,
                upgradeInitSpells,
                upgradeEnergyLevel,
                permanentTokens
            );
        }
    }
}