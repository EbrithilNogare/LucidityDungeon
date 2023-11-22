namespace Assets.Scripts
{
    struct MapTile
    {
        public Entries entries { get; private set; }
        public MapRoomContent roomContent { get; set; }

        public MapTile(Coordinate coordinate, GameState gameState, Config config)
        {
            entries = GetTileEntries(coordinate.x, coordinate.y, config);

            if (coordinate.x == 0 && coordinate.y == 0)
            {
                roomContent = MapRoomContent.Entry;
            }
            else
            {
                roomContent = GetMapContent(coordinate, gameState, config);
            }
        }

        private static MapRoomContent GetMapContent(Coordinate coordinate, GameState gameState, Config config)
        {
            MapRoomContent roomContent;

            float contentValue = (float)MyRandom.NextFloat(config.seed + coordinate.GetHashCode());

            if ((contentValue -= config.enemyAndTreasureProbability[gameState.upgradeEnemyAndTreasureProbability]) < 0)
            {
                roomContent = MapRoomContent.Enemy;
            }
            else if ((contentValue -= config.enemyAndTreasureProbability[gameState.upgradeEnemyAndTreasureProbability]) < 0)
            {
                roomContent = MapRoomContent.Treasure;
            }
            else if ((contentValue -= config.traderProbability) < 0)
            {
                roomContent = MapRoomContent.Trader;
            }
            else
            {
                roomContent = MapRoomContent.Empty;
            }

            return roomContent;
        }

        private static Entries GetTileEntries(int x, int y, Config config)
        {
            if (x == 0 && y == 0)
            {
                return new Entries(true, true, true, true);
            }

            bool up = MyRandom.NextFloat(config.seed + (x * 23 + y * 13)) < config.entryProbability;
            bool left = MyRandom.NextFloat(config.seed + (x * 23 + y * 13 + 7)) < config.entryProbability;
            bool down = MyRandom.NextFloat(config.seed + (x * 23 + (y - 1) * 13)) < config.entryProbability;
            bool right = MyRandom.NextFloat(config.seed + ((x + 1) * 23 + y * 13 + 7)) < config.entryProbability;

            if (x == 0 && y == -1) up = true;
            if (x == 1 && y == 0) left = true;
            if (x == 0 && y == 1) down = true;
            if (x == -1 && y == 0) right = true;

            return new Entries(up, left, down, right);
        }
    }

}
