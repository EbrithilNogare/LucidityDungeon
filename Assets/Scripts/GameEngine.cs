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
            turnState = new TurnState(turnState);

            if (turnState.energy-- <= 0)
            {
                doExit(turnState, false);
                return turnState;
            }

            switch (gameAction)
            {
                case GameAction.GoUp:
                case GameAction.GoLeft:
                case GameAction.GoDown:
                case GameAction.GoRight: doMove(turnState, gameAction); break;
                case GameAction.Attack: doAttack(turnState); break;
                case GameAction.DrinkPotion: doDrinkPotion(turnState); break;
                case GameAction.UseSpell: doUseSpell(turnState); break;
                case GameAction.OpenChest: doOpenChest(turnState); break;
                case GameAction.BuyPotion: doBuyPotion(turnState); break;
                case GameAction.BuySpell: doBuySpell(turnState); break;
                case GameAction.BuyToken: doBuyToken(turnState); break;
                case GameAction.Exit: doExit(turnState, true); break;
            }

            return turnState;
        }

        private void doExit(TurnState turnState, bool success)
        {
            if(success && (turnState.position.x != 0 || turnState.position.y != 0))
            {
                return;
            }
        }

        private void doAttack(TurnState turnState)
        {
            if (!isEnemyInMyRoom(turnState))
            {
                return;
            }

            int enemyLevel = GetEnemyLevel(turnState.position, gameState.upgradeEnemyLevel);

            Random random = new Random(config.seed + turnState.position.GetHashCode() + turnState.enemyHp);
            turnState.enemyHp -= random.Next(1, config.weaponDamageDiceSides[turnState.weaponLevel] + 1);

            if (turnState.enemyHp <= 0)
            {
                enemyDefeated(turnState, enemyLevel);
                return;
            }

            turnState.hp -= random.Next(1, config.enemyDamageCountPerLevel*enemyLevel + 5 + 1);

            if (turnState.hp <= 0)
            {
                doExit(turnState, false);
                return;
            }

        }

        private void enemyDefeated(TurnState turnState, int enemyLevel)
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
                    turnState.weaponLevel = Math.Max(turnState.weaponLevel, 1);
                }
                else if (enemyLevel < 7)
                {
                    turnState.weaponLevel = Math.Max(turnState.weaponLevel, 2);
                }
                else
                {
                    turnState.weaponLevel = Math.Max(turnState.weaponLevel, 3);
                }
            }

            turnState.money += config.enemyDropMoneyCountPerLevel * enemyLevel;
        }

        private int GetEnemyLevel(Coordinate position, int upgradeEnemyLevel)
        {
            Random random = new Random(config.seed + position.GetHashCode());
            return random.Next(config.enemyLevelRanges[upgradeEnemyLevel,0], config.enemyLevelRanges[upgradeEnemyLevel,1] + 1);
        }

        private void doBuyToken(TurnState turnState)
        {
            if (!isTraderInMyRoom(turnState) || turnState.money < config.tokenPrice)
            {
                return;
            }

            turnState.money -= config.tokenPrice;
            turnState.tokens += config.tokensCountForPrice;
        }

        private void doBuySpell(TurnState turnState)
        {
            if (!isTraderInMyRoom(turnState) || turnState.money < config.spellPrice)
            {
                return;
            }

            turnState.money -= config.spellPrice;
            turnState.spells++;
        }

        private void doBuyPotion(TurnState turnState)
        {
            if (!isTraderInMyRoom(turnState) || turnState.money < config.potionPrice)
            {
                return;
            }

            turnState.money -= config.potionPrice;
            turnState.potions++;
        }

        private void doOpenChest(TurnState turnState)
        {
            if (!isTreasureInMyRoom(turnState) || turnState.keys <= 0)
            {
                return;
            }

            Random random = new Random(config.seed + turnState.position.GetHashCode());

            turnState.keys--;
            turnState.money += random.Next(config.treasureDropMoneyCountMin, config.treasureDropMoneyCountMax + 1);
            turnState.tokens += random.Next(config.treasureDropTokensCountMin, config.treasureDropTokensCountMax + 1);
            turnState.treasureTaken.Add(turnState.position);
        }

        private void doUseSpell(TurnState turnState)
        {
            if (!isEnemyInMyRoom(turnState) || turnState.spells <= 0)
            {
                return;
            }

            turnState.spells--;
            turnState.enemyHp -= config.spellDamageIncrease[gameState.upgradeSpellLevel];
            if (turnState.enemyHp <= 0)
            {
                int enemyLevel = GetEnemyLevel(turnState.position, gameState.upgradeEnemyLevel);
                enemyDefeated(turnState, enemyLevel);
            }
        }

        private void doDrinkPotion(TurnState turnState)
        {
            if (turnState.potions <= 0)
            {
                return;
            }

            turnState.potions--;
            turnState.hp += config.healthPotionRegeneration[gameState.upgradePotionLevel];
            turnState.hp = Math.Min(turnState.hp, 100);
        }

        private void doMove(TurnState turnState, GameAction gameAction)
        {
            if (isEnemyInMyRoom(turnState))
            {
                return;
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
        }

        [Pure]
        private bool isTraderInMyRoom(TurnState turnState)
        {
            bool tileFound = map.TryGetValue(turnState.position, out MapTile mapTile);
            if (!tileFound) throw new Exception("not found tile I am standing on");
            return mapTile.roomContent == MapRoomContent.Trader;
        }

        [Pure]
        private bool isEnemyInMyRoom(TurnState turnState)
        {
            bool tileFound = map.TryGetValue(turnState.position, out MapTile mapTile);
            if (!tileFound) throw new Exception("not found tile I am standing on");
            return mapTile.roomContent == MapRoomContent.Enemy && !turnState.enemyDefeated.Contains(turnState.position);
        }

        [Pure]
        private bool isTreasureInMyRoom(TurnState turnState)
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
    }
}
