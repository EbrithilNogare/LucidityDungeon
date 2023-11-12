namespace Assets.Scripts
{
    class GameState
    {
        public int upgradeEnemyAndTreasureProbability;
        public int upgradeEnemyLevel;
        public int upgradePotionLevel;
        public int upgradeInitPotions;
        public int upgradeSpellLevel;
        public int upgradeInitSpells;
        public int upgradeEnergyLevel;
        public int lastRunTokens;

        public GameState()
        {
            upgradeEnemyAndTreasureProbability = 0;
            upgradeEnemyLevel = 0;
            upgradePotionLevel = 0;
            upgradeInitPotions = 0;
            upgradeSpellLevel = 0;
            upgradeInitSpells = 0;
            upgradeEnergyLevel = 0;
            lastRunTokens = 0;
        }

        public GameState(
            int upgradeEnemyAndTreasureProbability,
            int upgradeEnemyLevel,
            int upgradePotionLevel,
            int upgradeInitPotions,
            int upgradeSpellLevel,
            int upgradeInitSpells,
            int upgradeEnergyLevel,
            int lastRunTokens
        )
        {
            this.upgradeEnemyAndTreasureProbability = upgradeEnemyAndTreasureProbability;
            this.upgradeEnemyLevel = upgradeEnemyLevel;
            this.upgradePotionLevel = upgradePotionLevel;
            this.upgradeInitPotions = upgradeInitPotions;
            this.upgradeSpellLevel = upgradeSpellLevel;
            this.upgradeInitSpells = upgradeInitSpells;
            this.upgradeEnergyLevel = upgradeEnergyLevel;
            this.lastRunTokens = lastRunTokens;
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
            lastRunTokens = gameState.lastRunTokens;
        }
    }
}
