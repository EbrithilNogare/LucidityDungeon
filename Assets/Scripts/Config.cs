namespace Assets.Scripts
{
    class Config
    {
        public int seed = 5393; // balanced prime number

        public int[] energy = new int[] { 60, 90, 120, 160, 200 };

        // Map
        public float[] enemyAndTreasureProbability = new float[] { .1f, .14f, .18f, .22f, .26f };
        public float traderProbability = .05f;
        public float entryProbability = .7f;

        // Shoping hall
        public int[] enemyProbabilityPrices = new int[] { 0, 15, 30, 50, 80 };
        public int[] enemyLevelPrices = new int[] { 0, 25, 50, 75, 100 };
        public int[] upgradePotionPrices = new int[] { 0, 25, 45 };
        public int[] upgradeInitPotionsPrices = new int[] { 0, 40, 55 };
        public int[] upgradeSpellPrices = new int[] { 0, 25, 45 };
        public int[] upgradeInitSpellsPrices = new int[] { 0, 40, 55 };
        public int[] energyPrices = new int[] { 0, 10, 25, 55, 90 };

        // Player
        public int playerDefaultHealthPoints = 100;
        public int playerDefaultLives = 3;

        // Trader
        public int potionPrice = 2;
        public int spellPrice = 4;
        public int tokenPrice = 20;
        public int tokensCountForPrice = 2;

        // Enemy
        public int[,] enemyLevelRanges = { { 1, 3 }, { 1, 5 }, { 2, 7 }, { 3, 9 }, { 4, 10 } };
        public float enemyDropRateKeyPerLevel = .05f;
        public float enemyDropRateKeyBase = .33f;
        public float enemyDropRateWeapon = .33f;
        public int enemyDropMoneyCountPerLevel = 2;
        public int enemyDamageCountPerLevel = 5;
        public int enemyHealthCountPerLevel = 15;
        public int enemyHealthCountBase = 30;

        // Weapon
        public int[] weaponDamageDiceSides = { 10, 15, 20, 25 };

        // Potion
        public int[] healthPotionRegeneration = { 20, 40, 60 };

        // Spell
        public int[] spellDamageIncrease = { 20, 40, 60 };

        // Treasure
        public int treasureDropMoneyCountMin = 10;
        public int treasureDropMoneyCountMax = 20;
        public int treasureDropTokensCountMin = 10;
        public int treasureDropTokensCountMax = 30;
    }
}
