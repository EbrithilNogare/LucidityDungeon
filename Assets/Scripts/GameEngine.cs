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
        readonly public Dictionary<Coordinate, MapTile> map;

        public GameEngine(Config config)
        {
            gameState = new GameState();
            turnState = new TurnState(gameState, config);
            this.config = config;
            map = new Dictionary<Coordinate, MapTile>();
            map.Add(new Coordinate(0, 0), new MapTile(new Coordinate(0, 0), gameState, config));
        }

        public void Tick(GameAction gameAction)
        {
            turnState = Simulate(turnState, gameAction);
        }

        [Pure]
        public TurnState Simulate(TurnState turnState, GameAction gameAction)
        {
            if (turnState.energy-- <= 0)
            {
                return doExit(turnState, false); ;
            }

            switch (gameAction)
            {
                case GameAction.GoUp:
                case GameAction.GoLeft:
                case GameAction.GoDown:
                case GameAction.GoRight: turnState = doMove(turnState, gameAction); break;
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
            if (success && (turnState.position.x != 0 || turnState.position.y != 0))
            {
                gameState.permanentTokens = turnState.tokens;
                return new TurnState(gameState, config);
            }

            return new TurnState(gameState, config);
        }

        [Pure]
        private TurnState doAttack(TurnState turnState)
        {
            if (!isEnemyInMyRoom(turnState))
            {
                throw new Exception("Invalid action");
            }

            int enemyLevel = GetEnemyLevel(turnState.position, gameState.upgradeEnemyLevel);

            Random random = new Random(config.seed + turnState.position.GetHashCode() + turnState.enemyHp);
            turnState.enemyHp -= random.Next(1, config.weaponDamageDiceSides[turnState.weaponLevel] + 1);

            if (turnState.enemyHp <= 0)
            {
                enemyDefeated(turnState, enemyLevel);
                return turnState;
            }

            turnState.hp -= random.Next(1, config.enemyDamageCountPerLevel * enemyLevel + 5 + 1);

            if (turnState.hp <= 0)
            {
                return doExit(turnState, false); ;
            }

            return turnState;
        }

        [Pure]
        private TurnState enemyDefeated(TurnState turnState, int enemyLevel)
        {
            Random random = new Random(config.seed + turnState.position.GetHashCode());

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

            turnState.money += config.enemyDropMoneyCountPerLevel * enemyLevel;
            return turnState;
        }

        [Pure]
        private int GetEnemyLevel(Coordinate position, int upgradeEnemyLevel)
        {
            Random random = new Random(config.seed + position.GetHashCode());
            return random.Next(config.enemyLevelRanges[upgradeEnemyLevel, 0], config.enemyLevelRanges[upgradeEnemyLevel, 1] + 1);
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

            Random random = new Random(config.seed + turnState.position.GetHashCode());

            turnState.keys--;
            turnState.money += random.Next(config.treasureDropMoneyCountMin, config.treasureDropMoneyCountMax + 1);
            turnState.tokens += random.Next(config.treasureDropTokensCountMin, config.treasureDropTokensCountMax + 1);
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
                int enemyLevel = GetEnemyLevel(turnState.position, gameState.upgradeEnemyLevel);
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
                turnState.enemyHp = GetEnemyLevel(turnState.position, gameState.upgradeEnemyLevel) * config.enemyHealthCountPerLevel + config.enemyHealthCountBase;
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
    }
}
