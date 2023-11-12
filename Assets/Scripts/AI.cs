using System.Collections.Generic;

namespace Assets.Scripts
{
    class AI
    {
        System.Random random;
        GameAction lastMove = GameAction.Exit;

        public AI()
        {
            random = new System.Random(42);
        }

        public GameAction NextMove(GameEngine gameEngine)
        {
            List<GameAction> possibleMoves = gameEngine.GetValidActions();

            if (possibleMoves.Contains(GameAction.BuyToken)) return GameAction.BuyToken;
            if (possibleMoves.Contains(GameAction.OpenChest)) return GameAction.OpenChest;

            var distanceAndActionToExit = gameEngine.DistanceToExit(gameEngine.turnState);
            if (!possibleMoves.Contains(GameAction.Attack) && gameEngine.turnState.energy - 5 <= distanceAndActionToExit.Item1)
            {
                return distanceAndActionToExit.Item2;
            }

            possibleMoves.Remove(GameAction.Exit);

            var firstTry = possibleMoves[random.Next(0, possibleMoves.Count)];

            switch (firstTry)
            {
                case GameAction.GoDown:
                    if (lastMove != GameAction.GoUp) { lastMove = GameAction.GoDown; return GameAction.GoDown; } else break;
                case GameAction.GoUp:
                    if (lastMove != GameAction.GoDown) { lastMove = GameAction.GoUp; return GameAction.GoUp; } else break;
                case GameAction.GoRight:
                    if (lastMove != GameAction.GoLeft) { lastMove = GameAction.GoRight; return GameAction.GoRight; } else break;
                case GameAction.GoLeft:
                    if (lastMove != GameAction.GoRight) { lastMove = GameAction.GoLeft; return GameAction.GoLeft; } else break;
                default:
                    return firstTry;
            }

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
