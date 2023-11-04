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
            turnState = NewGame();
        }

        public TurnState NewGame()
        {
            map = new Dictionary<Coordinate, MapTile>();
            map.Add(new Coordinate(0, 0), new MapTile(new Coordinate(0, 0), gameState, config));
            return new TurnState(gameState, config);
        }

        public void Tick(GameAction gameAction)
        {
            turnState = Simulate(turnState, gameAction);
        }

        [Pure]
        public TurnState Simulate(TurnState turnState, GameAction gameAction)
        {
            if (turnState.energy <= 0)
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

        [Pure]
        private TurnState doExit(TurnState turnState, bool success)
        {
            if (success && (turnState.position.x == 0 || turnState.position.y == 0))
            {
                gameState.permanentTokens = turnState.tokens;
            }

            return NewGame();
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
            Random random = new Random(config.seed + turnState.position.GetHashCode());
            // warm up
            random.NextDouble();
            random.NextDouble();

            turnState.enemyDefeated.Add(turnState.position);

            if (random.NextDouble() < .1 * enemyLevel)
            {
                turnState.keys++;
            }

            if (random.NextDouble() < config.enemyDropRateWeapon)
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
            turnState.treasureTaken.Add(turnState.position);
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
                enemyDefeated(turnState, enemyLevel);
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

        [Pure]
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
            return mapTile.roomContent == MapRoomContent.Enemy && !turnState.enemyDefeated.Contains(turnState.position);
        }

        [Pure]
        public bool isTreasureInMyRoom(TurnState turnState)
        {
            bool tileFound = map.TryGetValue(turnState.position, out MapTile mapTile);
            if (!tileFound) throw new Exception("not found tile I am standing on");
            return mapTile.roomContent == MapRoomContent.Treasure && !turnState.treasureTaken.Contains(turnState.position);
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

            if (isTraderInMyRoom(turnState) && turnState.keys > 0)
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

        // Distance, BestMove
        [Pure]
        public Tuple<int, GameAction> DistanceToExit(TurnState turnState)
        {
            var checkOnMap = new List<Coordinate>() { new Coordinate(0, 0) };
            var distances = new Dictionary<Coordinate, Tuple<int, GameAction>>();
            distances.Add(new Coordinate(0, 0), new Tuple<int, GameAction>(0, GameAction.Exit));

            while (checkOnMap.Count > 0)
            {
                var tile = checkOnMap[0];
                checkOnMap.RemoveAt(0);

                if (tile.Equals(turnState.position))
                {
                    return distances[tile];
                }

                if (map.ContainsKey(tile) && map[tile].entries.up)
                {
                    var newTile = new Coordinate(tile);
                    newTile.y++;
                    if (distances.TryAdd(newTile, new Tuple<int, GameAction>(distances[tile].Item1 + 1, GameAction.GoDown)))
                    {
                        checkOnMap.Add(newTile);
                    }
                }
                if (map.ContainsKey(tile) && map[tile].entries.left)
                {
                    var newTile = new Coordinate(tile);
                    newTile.x--;
                    if (distances.TryAdd(newTile, new Tuple<int, GameAction>(distances[tile].Item1 + 1, GameAction.GoRight)))
                    {
                        checkOnMap.Add(newTile);
                    }
                }
                if (map.ContainsKey(tile) && map[tile].entries.down)
                {
                    var newTile = new Coordinate(tile);
                    newTile.y--;
                    if (distances.TryAdd(newTile, new Tuple<int, GameAction>(distances[tile].Item1 + 1, GameAction.GoUp)))
                    {
                        checkOnMap.Add(newTile);
                    }
                }
                if (map.ContainsKey(tile) && map[tile].entries.right)
                {
                    var newTile = new Coordinate(tile);
                    newTile.x++;
                    if (distances.TryAdd(newTile, new Tuple<int, GameAction>(distances[tile].Item1 + 1, GameAction.GoLeft)))
                    {
                        checkOnMap.Add(newTile);
                    }
                }
            }

            throw new Exception("BFS too long");
        }
    }
}
