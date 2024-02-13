using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Assets.Scripts
{
    class GameEngine
    {
        public GameState gameState;
        public TurnState turnState;
        public Config config;
        public Dictionary<Coordinate, MapTile> map;

        public GameEngine(Config config)
        {
            gameState = new GameState();
            this.config = config;
            NewGame();
        }

        public GameEngine(Config config, GameState gameState)
        {
            this.gameState = gameState;
            this.config = config;
            NewGame();
        }

        public void NewGame()
        {
            map = new Dictionary<Coordinate, MapTile>();
            map.Add(new Coordinate(0, 0), new MapTile(new Coordinate(0, 0), gameState, config));
            turnState = new TurnState(gameState, config);
        }

        public void Tick(GameAction gameAction)
        {
            turnState = Simulate(turnState, gameAction);
        }

        public TurnState Simulate(TurnState turnState, GameAction gameAction)
        {
            if (turnState.energy <= 0 || turnState.lives == 0)
            {
                return doExit(turnState, false); ;
            }

            switch (gameAction)
            {
                case GameAction.GoUp:
                case GameAction.GoLeft:
                case GameAction.GoDown:
                case GameAction.GoRight: turnState.energy--; turnState = doMove(turnState, gameAction); break;
                case GameAction.Attack: turnState = doAttack(turnState); break;
                case GameAction.UsePotion: turnState = doDrinkPotion(turnState); break;
                case GameAction.UseSpell: turnState = doUseSpell(turnState); break;
                case GameAction.OpenChest: turnState = doOpenChest(turnState); break;
                case GameAction.BuyPotion: turnState = doBuyPotion(turnState); break;
                case GameAction.BuySpell: turnState = doBuySpell(turnState); break;
                case GameAction.BuyToken: turnState = doBuyToken(turnState); break;
                case GameAction.Exit: turnState = doExit(turnState, true); break;
            }

            return turnState;
        }


        private TurnState doExit(TurnState turnState, bool success)
        {
            if (success && (turnState.position.x == 0 || turnState.position.y == 0))
            {
                gameState.lastRunTokens = turnState.tokens;
            }
            turnState.lives = 0;
            return turnState;
        }

        [Pure]
        private TurnState doAttack(TurnState turnState)
        {
            if (!isEnemyInMyRoom(turnState))
            {
                throw new Exception("Invalid action");
            }

            int enemyLevel = GetEnemyLevel(turnState.position);


            turnState.enemyHp -= MyRandom.RangeInt(config.seed + turnState.position.GetHashCode() + turnState.enemyHp, 1, config.weaponDamageDiceSides[turnState.weaponLevel] + 1);

            if (turnState.enemyHp <= 0)
            {
                return enemyDefeated(turnState, enemyLevel);
            }

            turnState.hp -= MyRandom.RangeInt(config.seed + turnState.position.GetHashCode() + turnState.hp, 1, config.enemyDamageCountPerLevel * enemyLevel + 5 + 1);

            if (turnState.hp <= 0)
            {
                turnState.lives--;
                if (turnState.lives <= 0)
                {
                    return doExit(turnState, false); ;
                }
                else
                {
                    turnState.hp = config.playerDefaultHealthPoints;
                    // todo move player one field back
                }
            }

            return turnState;
        }

        [Pure]
        private TurnState enemyDefeated(TurnState turnState, int enemyLevel)
        {
            turnState.roomCleared.Add(turnState.position);

            if (MyRandom.NextFloat(config.seed + turnState.position.GetHashCode()) < config.enemyDropRateKeyBase + config.enemyDropRateKeyPerLevel * enemyLevel)
            {
                turnState.keys++;
            }

            if (MyRandom.NextFloat(config.seed + turnState.position.GetHashCode() + 1) < config.enemyDropRateWeapon)
            {
                if (enemyLevel < 4)
                {
                    turnState.weaponLevel = Math.Max(turnState.weaponLevel, (byte)1);
                }
                else if (enemyLevel < 7)
                {
                    turnState.weaponLevel = Math.Max(turnState.weaponLevel, (byte)2);
                }
                else
                {
                    turnState.weaponLevel = Math.Max(turnState.weaponLevel, (byte)3);
                }
            }

            turnState.hp = config.playerDefaultHealthPoints;
            turnState.money += config.enemyDropMoneyCountPerLevel * enemyLevel;
            return turnState;
        }

        [Pure]
        public int GetEnemyLevel(Coordinate position)
        {
            int upgradeEnemyLevel = gameState.upgradeEnemyLevel;
            return MyRandom.RangeInt(config.seed + position.GetHashCode(), config.enemyLevelRanges[upgradeEnemyLevel, 0], config.enemyLevelRanges[upgradeEnemyLevel, 1] + 1);
        }

        [Pure]
        private TurnState doBuyToken(TurnState turnState)
        {
            if (!isTraderInMyRoom(turnState) || turnState.money < config.tokenPrice)
            {
                throw new Exception("Invalid action");
            }

            turnState.money -= config.tokenPrice;
            turnState.tokens += config.tokensCountForPrice;
            return turnState;
        }

        [Pure]
        private TurnState doBuySpell(TurnState turnState)
        {
            if (!isTraderInMyRoom(turnState) || turnState.money < config.spellPrice)
            {
                throw new Exception("Invalid action");
            }

            turnState.money -= config.spellPrice;
            turnState.spells++;
            return turnState;
        }

        [Pure]
        private TurnState doBuyPotion(TurnState turnState)
        {
            if (!isTraderInMyRoom(turnState) || turnState.money < config.potionPrice)
            {
                throw new Exception("Invalid action");
            }

            turnState.money -= config.potionPrice;
            turnState.potions++;
            return turnState;
        }

        [Pure]
        private TurnState doOpenChest(TurnState turnState)
        {
            if (!isTreasureInMyRoom(turnState) || turnState.keys <= 0)
            {
                throw new Exception("Invalid action");
            }

            turnState.keys--;
            turnState.money += MyRandom.RangeInt(config.seed + turnState.position.GetHashCode(), config.treasureDropMoneyCountMin, config.treasureDropMoneyCountMax + 1);
            turnState.tokens += MyRandom.RangeInt(config.seed + turnState.position.GetHashCode() + 42, config.treasureDropTokensCountMin, config.treasureDropTokensCountMax + 1);
            turnState.roomCleared.Add(turnState.position);
            return turnState;
        }

        [Pure]
        private TurnState doUseSpell(TurnState turnState)
        {
            if (!isEnemyInMyRoom(turnState) || turnState.spells <= 0)
            {
                throw new Exception("Invalid action");
            }

            turnState.spells--;
            turnState.enemyHp -= config.spellDamageIncrease[gameState.upgradeSpellLevel];
            if (turnState.enemyHp <= 0)
            {
                int enemyLevel = GetEnemyLevel(turnState.position);
                return enemyDefeated(turnState, enemyLevel);
            }

            return turnState;
        }

        [Pure]
        private TurnState doDrinkPotion(TurnState turnState)
        {
            if (turnState.potions <= 0)
            {
                throw new Exception("Invalid action");
            }

            turnState.potions--;
            turnState.hp += config.healthPotionRegeneration[gameState.upgradePotionLevel];
            turnState.hp = Math.Min(turnState.hp, config.playerDefaultHealthPoints);
            return turnState;
        }

        private TurnState doMove(TurnState turnState, GameAction gameAction)
        {
            if (isEnemyInMyRoom(turnState))
            {
                throw new Exception("Invalid action");
            }

            if (gameAction == GameAction.GoUp && map[turnState.position].entries.up)
            {
                turnState.position.y++;
                checkMapTile(turnState.position);
            }

            if (gameAction == GameAction.GoLeft && map[turnState.position].entries.left)
            {
                turnState.position.x--;
                checkMapTile(turnState.position);
            }

            if (gameAction == GameAction.GoDown && map[turnState.position].entries.down)
            {
                turnState.position.y--;
                checkMapTile(turnState.position);
            }

            if (gameAction == GameAction.GoRight && map[turnState.position].entries.right)
            {
                turnState.position.x++;
                checkMapTile(turnState.position);
            }

            if (isEnemyInMyRoom(turnState))
            {
                turnState.enemyHp = GetEnemyLevel(turnState.position) * config.enemyHealthCountPerLevel + config.enemyHealthCountBase;
                turnState.hp = config.playerDefaultHealthPoints;
            }

            return turnState;
        }

        [Pure]
        public bool isTraderInMyRoom(TurnState turnState)
        {
            bool tileFound = map.TryGetValue(turnState.position, out MapTile mapTile);
            if (!tileFound) throw new Exception("not found tile I am standing on");
            return mapTile.roomContent == MapRoomContent.Trader;
        }

        [Pure]
        public bool isEnemyInMyRoom(TurnState turnState)
        {
            bool tileFound = map.TryGetValue(turnState.position, out MapTile mapTile);
            if (!tileFound) throw new Exception("not found tile I am standing on");
            return mapTile.roomContent == MapRoomContent.Enemy && !turnState.roomCleared.Contains(turnState.position);
        }

        [Pure]
        public bool isTreasureInMyRoom(TurnState turnState)
        {
            bool tileFound = map.TryGetValue(turnState.position, out MapTile mapTile);
            if (!tileFound) throw new Exception("not found tile I am standing on");
            return mapTile.roomContent == MapRoomContent.Treasure && !turnState.roomCleared.Contains(turnState.position);
        }

        public void checkMapTile(Coordinate coordinate)
        {
            if (!map.ContainsKey(coordinate))
            {
                map.Add(coordinate, new MapTile(coordinate, gameState, config));
            }
        }

        [Pure]
        public List<GameAction> GetValidActions()
        {
            List<GameAction> actions = new List<GameAction>();

            if (isTraderInMyRoom(turnState))
            {
                if (turnState.money >= config.potionPrice)
                {
                    actions.Add(GameAction.BuyPotion);
                }
                if (turnState.money >= config.spellPrice)
                {
                    actions.Add(GameAction.BuySpell);
                }
                if (turnState.money >= config.tokenPrice)
                {
                    actions.Add(GameAction.BuyToken);
                }
            }
            if (isTreasureInMyRoom(turnState) && turnState.keys > 0)
            {
                actions.Add(GameAction.OpenChest);
            }
            if (isEnemyInMyRoom(turnState))
            {
                actions.Add(GameAction.Attack);
                if (turnState.spells > 0)
                {
                    actions.Add(GameAction.UseSpell);
                }
                if (turnState.potions > 0)
                {
                    actions.Add(GameAction.UsePotion);
                }
            }
            else
            {

                if (map[turnState.position].entries.up)
                {
                    actions.Add(GameAction.GoUp);
                }
                if (map[turnState.position].entries.left)
                {
                    actions.Add(GameAction.GoLeft);
                }
                if (map[turnState.position].entries.down)
                {
                    actions.Add(GameAction.GoDown);
                }
                if (map[turnState.position].entries.right)
                {
                    actions.Add(GameAction.GoRight);
                }
            }
            if (turnState.position.x == 0 && turnState.position.y == 0)
            {
                actions.Add(GameAction.Exit);
            }

            return actions;
        }

        [Pure]
        public List<GameAction> GetPathFromSourceToGoal(Coordinate source, Coordinate goal)
        {
            var distances = new Dictionary<Coordinate, List<GameAction>>
            {
                { source, new List<GameAction>(new List<GameAction>() { }) }
            };
            var toCheck = new List<Coordinate>() { source };

            while (toCheck.Count > 0 && toCheck.Count < 1000)
            {
                var tile = toCheck[0];
                toCheck.RemoveAt(0);

                if (tile.Equals(goal))
                {
                    return distances[tile];
                }

                if (map.ContainsKey(tile) && map[tile].entries.up)
                {
                    var newTile = new Coordinate(tile);
                    newTile.y++;
                    if (distances.TryAdd(newTile, new List<GameAction>(distances[tile])))
                    {
                        distances[newTile].Add(GameAction.GoUp);
                        toCheck.Add(newTile);
                    }
                }
                if (map.ContainsKey(tile) && map[tile].entries.left)
                {
                    var newTile = new Coordinate(tile);
                    newTile.x--;
                    if (distances.TryAdd(newTile, new List<GameAction>(distances[tile])))
                    {
                        distances[newTile].Add(GameAction.GoLeft);
                        toCheck.Add(newTile);
                    }
                }
                if (map.ContainsKey(tile) && map[tile].entries.down)
                {
                    var newTile = new Coordinate(tile);
                    newTile.y--;
                    if (distances.TryAdd(newTile, new List<GameAction>(distances[tile])))
                    {
                        distances[newTile].Add(GameAction.GoDown);
                        toCheck.Add(newTile);
                    }
                }
                if (map.ContainsKey(tile) && map[tile].entries.right)
                {
                    var newTile = new Coordinate(tile);
                    newTile.x++;
                    if (distances.TryAdd(newTile, new List<GameAction>(distances[tile])))
                    {
                        distances[newTile].Add(GameAction.GoRight);
                        toCheck.Add(newTile);
                    }
                }
            }

            throw new Exception("BFS too long");
        }

        // Distance, BestMove
        [Pure]
        public Tuple<int, GameAction> DistanceToExit(TurnState turnState)
        {
            List<GameAction> pathToStart = GetPathFromSourceToGoal(turnState.position, new Coordinate(0, 0));
            if (pathToStart.Count == 0)
            {
                return new Tuple<int, GameAction>(pathToStart.Count, GameAction.Exit);
            }
            else
            {
                return new Tuple<int, GameAction>(pathToStart.Count, pathToStart[0]);
            }
        }

        public List<ShoppingHallAction> GetValidActionsInShoppingHall()
        {
            var tokens = gameState.lastRunTokens;
            List<ShoppingHallAction> output = new List<ShoppingHallAction>();

            if (gameState.upgradeEnemyAndTreasureProbability + 1 < config.enemyProbabilityPrices.Length
                && tokens >= config.enemyProbabilityPrices[gameState.upgradeEnemyAndTreasureProbability + 1])
            { output.Add(ShoppingHallAction.upgradeEnemyAndTreasureProbability); }

            if (gameState.upgradeEnemyLevel + 1 < config.enemyLevelPrices.Length
                && tokens >= config.enemyLevelPrices[gameState.upgradeEnemyLevel + 1])
            { output.Add(ShoppingHallAction.upgradeEnemyLevel); }

            if (gameState.upgradePotionLevel + 1 < config.upgradePotionPrices.Length
                && tokens >= config.upgradePotionPrices[gameState.upgradePotionLevel + 1])
            { output.Add(ShoppingHallAction.upgradePotionLevel); }

            if (gameState.upgradeInitPotions + 1 < config.upgradeInitPotionsPrices.Length
                && tokens >= config.upgradeInitPotionsPrices[gameState.upgradeInitPotions + 1])
            { output.Add(ShoppingHallAction.upgradeInitPotions); }

            if (gameState.upgradeSpellLevel + 1 < config.upgradeSpellPrices.Length
                && tokens >= config.upgradeSpellPrices[gameState.upgradeSpellLevel + 1])
            { output.Add(ShoppingHallAction.upgradeSpellLevel); }

            if (gameState.upgradeInitSpells + 1 < config.upgradeInitSpellsPrices.Length
                && tokens >= config.upgradeInitSpellsPrices[gameState.upgradeInitSpells + 1])
            { output.Add(ShoppingHallAction.upgradeInitSpells); }

            if (gameState.upgradeEnergyLevel + 1 < config.energyPrices.Length
                && tokens >= config.energyPrices[gameState.upgradeEnergyLevel + 1])
            { output.Add(ShoppingHallAction.upgradeEnergyLevel); }

            return output;
        }

        public void BuyInShoppingHall(ShoppingHallAction action)
        {
            if (!GetValidActionsInShoppingHall().Contains(action))
            {
                throw new Exception("invalid shoppinghall action: " + action.ToString());
            }

            switch (action)
            {
                case ShoppingHallAction.upgradeEnemyAndTreasureProbability:
                    gameState.lastRunTokens -= config.enemyProbabilityPrices[gameState.upgradeEnemyAndTreasureProbability + 1];
                    gameState.upgradeEnemyAndTreasureProbability++;
                    break;
                case ShoppingHallAction.upgradeEnemyLevel:
                    gameState.lastRunTokens -= config.enemyLevelPrices[gameState.upgradeEnemyLevel + 1];
                    gameState.upgradeEnemyLevel++;
                    break;
                case ShoppingHallAction.upgradePotionLevel:
                    gameState.lastRunTokens -= config.upgradePotionPrices[gameState.upgradePotionLevel + 1];
                    gameState.upgradePotionLevel++;
                    break;
                case ShoppingHallAction.upgradeInitPotions:
                    gameState.lastRunTokens -= config.upgradeInitPotionsPrices[gameState.upgradeInitPotions + 1];
                    gameState.upgradeInitPotions++;
                    break;
                case ShoppingHallAction.upgradeSpellLevel:
                    gameState.lastRunTokens -= config.upgradeSpellPrices[gameState.upgradeSpellLevel + 1];
                    gameState.upgradeSpellLevel++;
                    break;
                case ShoppingHallAction.upgradeInitSpells:
                    gameState.lastRunTokens -= config.upgradeInitSpellsPrices[gameState.upgradeInitSpells + 1];
                    gameState.upgradeInitSpells++;
                    break;
                case ShoppingHallAction.upgradeEnergyLevel:
                    gameState.lastRunTokens -= config.energyPrices[gameState.upgradeEnergyLevel + 1];
                    gameState.upgradeEnergyLevel++;
                    break;
            }

            if (gameState.lastRunTokens < 0)
            {
                throw new Exception("lastRunTokens < 0");
            }
        }
    }
}
