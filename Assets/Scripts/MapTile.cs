using System;
using System.Collections.Generic;
using System.Text;

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

            Random random = new Random(config.seed + coordinate.GetHashCode());
            float contentValue = (float)random.NextDouble();

            if ((contentValue -= config.enemyProbability[gameState.upgradeEnemyAndTreasureProbability]) < 0)
            {
                roomContent = MapRoomContent.Enemy;
            }
            else if ((contentValue -= config.enemyProbability[gameState.upgradeEnemyAndTreasureProbability]) < 0)
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

            bool up = new Random(config.seed + x * 733 + y * 739).NextDouble() < config.entryProbability;
            bool left = new Random(config.seed + x * 733 + y * 739 + 7).NextDouble() < config.entryProbability;
            bool down = new Random(config.seed + x * 733 + (y + 1) * 739).NextDouble() < config.entryProbability;
            bool right = new Random(config.seed + (x + 1) * 733 + y * 739 + 7).NextDouble() < config.entryProbability;

            if (x == 0 && y == -1) up = true;
            if (x == 1 && y == 0) left = true;
            if (x == 0 && y == 1) down = true;
            if (x == -1 && y == 0) right = true;

            return new Entries(up, left, down, right);
        }
    }

}
