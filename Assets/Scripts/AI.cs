using System.Collections.Generic;

namespace Assets.Scripts
{
    class AI
    {
        System.Random random;

        public AI()
        {
            random = new System.Random(42);
        }

        public GameAction NextMove(GameEngine gameEngine)
        {
            List<GameAction> possibleMoves = gameEngine.GetValidActions();

            if (possibleMoves.Contains(GameAction.BuyToken)) return GameAction.BuyToken;
            if (possibleMoves.Contains(GameAction.OpenChest)) return GameAction.OpenChest;
            if (possibleMoves.Contains(GameAction.UseSpell) && gameEngine.turnState.hp < gameEngine.config.playerDefaultHealthPoints / 2) return GameAction.UseSpell;
            if (possibleMoves.Contains(GameAction.UsePotion)
                && gameEngine.turnState.hp + gameEngine.config.healthPotionRegeneration[gameEngine.gameState.upgradePotionLevel] < gameEngine.config.playerDefaultHealthPoints)
            {
                return GameAction.UsePotion;
            }
            if (possibleMoves.Contains(GameAction.Attack)) return GameAction.Attack;

            var distanceAndActionToExit = gameEngine.DistanceToExit(gameEngine.turnState);
            if (gameEngine.turnState.energy - 5 <= distanceAndActionToExit.Item1)
            {
                return distanceAndActionToExit.Item2;
            }

            possibleMoves.Remove(GameAction.Exit);

            var x = gameEngine.turnState.position.x;
            var y = gameEngine.turnState.position.y;
            var possibleGoings = new List<GameAction>();
            if (possibleMoves.Contains(GameAction.GoUp) && !gameEngine.map.ContainsKey(new Coordinate(x, y + 1)))
                possibleGoings.Add(GameAction.GoUp);
            if (possibleMoves.Contains(GameAction.GoLeft) && !gameEngine.map.ContainsKey(new Coordinate(x - 1, y)))
                possibleGoings.Add(GameAction.GoLeft);
            if (possibleMoves.Contains(GameAction.GoDown) && !gameEngine.map.ContainsKey(new Coordinate(x, y - 1)))
                possibleGoings.Add(GameAction.GoDown);
            if (possibleMoves.Contains(GameAction.GoRight) && !gameEngine.map.ContainsKey(new Coordinate(x + 1, y)))
                possibleGoings.Add(GameAction.GoRight);

            if (possibleGoings.Count > 0)
                return possibleGoings[random.Next(0, possibleGoings.Count)];

            return possibleMoves[random.Next(0, possibleMoves.Count)];
        }

        public ShoppingHallAction? NextShoppingHallAction(GameEngine gameEngine)
        {
            var possibleActions = gameEngine.GetValidActionsInShoppingHall();

            if (possibleActions.Count == 0)
                return null;

            return possibleActions[random.Next(0, possibleActions.Count)];
        }
    }
}
