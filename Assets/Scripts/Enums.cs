namespace Assets.Scripts
{
    enum GameAction
    {
        // Navigation
        GoUp,
        GoLeft,
        GoDown,
        GoRight,

        // Enemy actions
        Attack,
        UsePotion,
        UseSpell,

        // Treasure actions
        OpenChest,

        // Trader actions
        BuyPotion,
        BuySpell,
        BuyToken,

        // Entry actions
        Exit,
    }

    enum MapRoomContent
    {
        Entry,
        Empty,
        Enemy,
        Treasure,
        Trader,
    }

    enum RoomTileType
    {
        F,  // floorTiles
        W,  // wallTiles
        U,  // edgeUpTiles
        L,  // edgeLeftTiles;
        R,  // edgeRightTiles;
        UL, // edgeUpLeftTiles;
        UR, // edgeUpRightTiles;
        N,  // nothing
    }

    enum ShoppingHallAction
    {
        upgradeEnemyAndTreasureProbability,
        upgradeEnemyLevel,
        upgradePotionLevel,
        upgradeInitPotions,
        upgradeSpellLevel,
        upgradeInitSpells,
        upgradeEnergyLevel,
    }
}