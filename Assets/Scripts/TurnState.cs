using System.Collections.Generic;

namespace Assets.Scripts
{
    struct TurnState
    {
        public Coordinate position;
        public byte lives;
        public int hp;
        public int enemyHp;
        public byte weaponLevel;
        public byte energy;
        public int potions;
        public int spells;
        public int money;
        public int tokens;
        public int keys;
        public List<Coordinate> roomCleared;

        public TurnState(GameState gameState, Config config)
        {
            lives = (byte)config.playerDefaultLives;
            hp = 100;
            enemyHp = 0;
            weaponLevel = 0;
            energy = (byte)config.energy[gameState.upgradeEnergyLevel];
            position = new Coordinate(0, 0);
            potions = gameState.upgradeInitPotions;
            spells = gameState.upgradeInitSpells;
            money = 0;
            tokens = 0;
            keys = 0;
            roomCleared = new List<Coordinate>();
        }

        public TurnState(TurnState turnState)
        {
            lives = turnState.lives;
            hp = turnState.hp;
            enemyHp = turnState.enemyHp;
            weaponLevel = turnState.weaponLevel;
            energy = turnState.energy;
            position = new Coordinate(turnState.position);
            potions = turnState.potions;
            spells = turnState.spells;
            money = turnState.money;
            tokens = turnState.tokens;
            keys = turnState.keys;
            roomCleared = new List<Coordinate>(turnState.roomCleared);
        }
    }
}
