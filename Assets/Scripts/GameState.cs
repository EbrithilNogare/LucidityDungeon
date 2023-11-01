namespace Assets.Scripts
{
    class GameState
    {
        public int upgradeEnemyAndTreasureProbability;
        /// <summary>
        /// Range: 0 - 4
        /// </summary>
        public int upgradeEnemyLevel;
        public int upgradePotionLevel;
        public int upgradeInitPotions;
        public int upgradeSpellLevel;
        public int upgradeInitSpells;
        public int upgradeEnergyLevel;
        public int permanentTokens;

        public GameState()
        {
            upgradeEnemyAndTreasureProbability = 0;
            upgradeEnemyLevel = 0;
            upgradePotionLevel = 0;
            upgradeInitPotions = 0;
            upgradeSpellLevel = 0;
            upgradeInitSpells = 0;
            upgradeEnergyLevel = 0;
            permanentTokens = 0;
        }

        public GameState(
            int upgradeEnemyAndTreasureProbability,
            int upgradeEnemyLevel,
            int upgradePotionLevel,
            int upgradeInitPotions,
            int upgradeSpellLevel,
            int upgradeInitSpells,
            int upgradeEnergyLevel,
            int permanentTokens
        )
        {
            this.upgradeEnemyAndTreasureProbability = upgradeEnemyAndTreasureProbability;
            this.upgradeEnemyLevel = upgradeEnemyLevel;
            this.upgradePotionLevel = upgradePotionLevel;
            this.upgradeInitPotions = upgradeInitPotions;
            this.upgradeSpellLevel = upgradeSpellLevel;
            this.upgradeInitSpells = upgradeInitSpells;
            this.upgradeEnergyLevel = upgradeEnergyLevel;
            this.permanentTokens = permanentTokens;
        }

        public GameState(GameState gameState)
        {
            upgradeEnemyAndTreasureProbability = gameState.upgradeEnemyAndTreasureProbability;
            upgradeEnemyLevel = gameState.upgradeEnemyLevel;
            upgradePotionLevel = gameState.upgradePotionLevel;
            upgradeInitPotions = gameState.upgradeInitPotions;
            upgradeSpellLevel = gameState.upgradeSpellLevel;
            upgradeInitSpells = gameState.upgradeInitSpells;
            upgradeEnergyLevel = gameState.upgradeEnergyLevel;
            permanentTokens = gameState.permanentTokens;
        }
    }
}
