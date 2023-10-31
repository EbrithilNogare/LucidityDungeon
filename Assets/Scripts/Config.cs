namespace Assets.Scripts
{
    class Config
    {
        public int seed = 5393; // balanced prime number

        public int[] energy = new int[] { 30, 45, 60, 80, 100 };

        // Map
        public float[] enemyProbability = new float[] { .05f, .07f, .1f, .12f, .15f };
        public float[] treasureProbability = new float[] { .1f, .12f, .15f, .19f, .25f };
        public float traderProbability = .05f;
        public float entryProbability = .7f;

        // Shoping hall
        public int[] enemyProbabilityPrices = new int[] { 0, 15, 30, 50, 80 };
        public int[] enemyLevelPrices = new int[] { 0, 25, 50, 75, 100 };
        public int[] energyPrices = new int[] { 0, 10, 25, 55, 90 };
        public int tokensCountForPrice = 5;

        // Player
        public int playerDefaultHealthPoints = 100;

        // Trader
        public int potionPrice = 5;
        public int spellPrice = 2;
        public int tokenPrice = 50;

        // Enemy
        public int[,] enemyLevelRanges = { { 1, 3 }, { 1, 5 }, { 2, 7 }, { 3, 9 }, { 4, 10 } };
        public float enemyDropRateKeyPerLevel = .1f;
        public float enemyDropRateWeapon = .33f;
        public int enemyDropMoneyCountPerLevel = 2;
        public int enemyDamageCountPerLevel = 1;
        public int enemyHealthCountPerLevel = 10;
        public int enemyHealthCountBase = 20;

        // Weapon
        public int[] weaponDamageDiceSides = { 4, 6, 8, 10 };

        // Potion
        public int[] healthPotionRegeneration = { 25, 50, 100 };

        // Spell
        public int[] spellDamageIncrease = { 20, 40, 60 };

        // Treasure
        public int treasureDropMoneyCountMin = 10;
        public int treasureDropMoneyCountMax = 20;
        public int treasureDropTokensCountMin = 10;
        public int treasureDropTokensCountMax = 30;
    }
}
