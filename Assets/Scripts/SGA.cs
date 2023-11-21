using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class SGA : MonoBehaviour
    {
        static readonly int POP_SIZE = 32;
        static readonly int IND_LEN = 26;
        static readonly int MAX_GEN = 1;
        static readonly int MAX_VAL = 100;
        static readonly int MAX_RUNS = 60;
        static readonly double CX_PROB = 0.5;
        static readonly double MUT_PROB = 0.5;
        static readonly double MUT_FLIP_PROB = 1 / (float)IND_LEN;
        static System.Random random = new System.Random();

        static List<int> CreateIndividual(int length)
        {
            List<int> individual = new List<int>();
            for (int i = 0; i < length; i++)
            {
                individual.Add(random.Next((int)(3 / 8f * MAX_VAL), (int)(5 / 8f * MAX_VAL)));
            }
            return individual;
        }

        static List<List<int>> CreatePopulation(int size)
        {
            List<List<int>> population = new List<List<int>>();
            for (int i = 0; i < size / 2; i++)
            {
                population.Add(CreateIndividual(IND_LEN));
            }
            for (int i = 0; i < size - size / 2; i++)
            {
                // manual best
                population.Add(new List<int> { 63, 59, 45, 89, 62, 51, 30, 65, 38, 35, 57, 50, 61, 44, 55, 58, 40, 49, 29, 64, 54, 39, 65, 68, 56, 63 });
            }

            return population;
        }

        static List<List<int>> Selection(List<List<int>> population, List<int> fits)
        {
            List<List<int>> selected = new List<List<int>>();
            for (int i = 0; i < population.Count; i++)
            {
                int index1 = random.Next(population.Count);
                int index2 = random.Next(population.Count);
                int index3 = random.Next(population.Count);
                int index4 = random.Next(population.Count);
                if (fits[index1] >= fits[index2] && fits[index1] >= fits[index3] && fits[index1] >= fits[index4])
                {
                    selected.Add(new List<int>(population[index1]));
                }
                else if (fits[index2] >= fits[index1] && fits[index2] >= fits[index3] && fits[index2] >= fits[index4])
                {
                    selected.Add(new List<int>(population[index2]));
                }
                else if (fits[index3] >= fits[index1] && fits[index3] >= fits[index2] && fits[index3] >= fits[index4])
                {
                    selected.Add(new List<int>(population[index3]));
                }
                else
                {
                    selected.Add(new List<int>(population[index4]));
                }
            }
            return selected;
        }

        static Tuple<List<int>, List<int>> Crossover(List<int> p1, List<int> p2)
        {
            int point = random.Next(p1.Count);
            List<int> o1 = new List<int>(p1.Take(point).Concat(p2.Skip(point)));
            List<int> o2 = new List<int>(p2.Take(point).Concat(p1.Skip(point)));
            return Tuple.Create(o1, o2);
        }

        static List<List<int>> CrossoverPopulation(List<List<int>> population)
        {
            List<List<int>> offspring = new List<List<int>>();
            for (int i = 0; i + 1 < population.Count; i += 2)
            {
                List<int> p1 = new List<int>(population[i]);
                List<int> p2 = new List<int>(population[i + 1]);
                if (random.NextDouble() < CX_PROB)
                {
                    var children = Crossover(p1, p2);
                    offspring.Add(children.Item1);
                    offspring.Add(children.Item2);
                }
                else
                {
                    offspring.Add(p1);
                    offspring.Add(p2);
                }
            }
            return offspring;
        }

        static List<int> Mutate(List<int> individual)
        {
            if (random.NextDouble() < MUT_PROB)
            {
                for (int i = 0; i < individual.Count; i++)
                {
                    individual[i] += (random.NextDouble() < MUT_FLIP_PROB) ? random.Next(-MAX_VAL / 10, MAX_VAL / 10 + 1) : 0;
                    individual[i] = Math.Clamp(individual[i], 0, MAX_VAL);
                }
            }
            return individual;
        }

        static List<List<int>> MutationPopulation(List<List<int>> population)
        {
            for (int i = 0; i < population.Count; i++)
            {
                population[i] = Mutate(population[i]);
            }
            return population;
        }

        static List<List<int>> EvolutionaryAlgorithm(List<List<int>> population, Func<List<int>, int> fitnessFunction)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            List<Tuple<int, int, double, int>> log = new List<Tuple<int, int, double, int>>();
            for (int G = 0; G < MAX_GEN; G++)
            {
                stopwatch.Start();

                List<int> fits = new List<int>(POP_SIZE);
                object lockObject = new object();

                Parallel.ForEach(population, () => new List<int>(),
                (individual, loopState, localFits) =>
                {
                    localFits.Add(fitnessFunction(individual));
                    return localFits;
                },
                localFits =>
                {
                    lock (lockObject)
                    {
                        fits.AddRange(localFits);
                    }
                });

                stopwatch.Stop();

                Debug.Log("Generation: " + G);
                Debug.Log("Time for run: " + stopwatch.ElapsedMilliseconds / (float)MAX_GEN / (float)POP_SIZE / (float)MAX_RUNS);
                Debug.Log("fits.Max(): " + fits.Max());
                Debug.Log("{" + string.Join(", ", population[fits.IndexOf(fits.Max())]) + "}");
                List<List<int>> matingPool = Selection(population, fits);
                List<List<int>> offspring = MutationPopulation(CrossoverPopulation(matingPool));
                offspring[0] = new List<int>(population[fits.IndexOf(fits.Max())]);
                offspring[1] = new List<int>(population[fits.IndexOf(fits.Max())]);
                offspring[2] = new List<int>(population[fits.IndexOf(fits.Max())]);

                population = offspring;
            }
            return population;
        }

        static int FitnessFunction(List<int> individual)
        {
            Config config = MapToConfig(individual);
            config.seed = random.Next(10000);

            var gameEngine = new GameEngine(config);
            var ai = new AI();

            List<int> upgradeTimes = new List<int>();

            for (int i = 0; i < MAX_RUNS; i++)
            {
                GameAction action;

                gameEngine.NewGame();

                do
                {
                    action = ai.NextMove(gameEngine);
                    gameEngine.Tick(action);
                }
                while (action != GameAction.Exit && gameEngine.turnState.lives > 0);

                var nextShoppingHallAction = ai.NextShoppingHallAction(gameEngine);
                while (nextShoppingHallAction != null)
                {
                    upgradeTimes.Add(i);
                    gameEngine.BuyInShoppingHall(nextShoppingHallAction.Value);
                    nextShoppingHallAction = ai.NextShoppingHallAction(gameEngine);
                }
            }

            if (upgradeTimes.Count < 10) return upgradeTimes.Count;

            int penalty = 0;

            for (int i = 0; i < upgradeTimes.Count; i++)
            {
                penalty += (int)Math.Abs((MAX_RUNS - 10) / (float)upgradeTimes.Count * i - upgradeTimes[i]);
            }


            return upgradeTimes.Count * 1000 - penalty;
        }

        public static void Main()
        {
            List<List<int>> population = CreatePopulation(POP_SIZE);

            population = EvolutionaryAlgorithm(population, FitnessFunction);
        }

        public static IEnumerable<int> MainGenerator()
        {
            List<List<int>> population = CreatePopulation(POP_SIZE);
            int iterator = 0;
            while (true)
            {
                Debug.Log("start run: " + iterator++);
                population = EvolutionaryAlgorithm(population, FitnessFunction);
                yield return 0;
            }
        }

        [Pure]
        private static Config MapToConfig(List<int> individual)
        {
            Config config = new Config();
            int individualIterator = 0;
            float diffF = 0;
            int diffI = 0;

            // energy static

            config.enemyAndTreasureProbability[0] = Scale(individual[individualIterator++], .01f, .1f);
            diffF = Scale(individual[individualIterator++], .01f, .1f);
            config.enemyAndTreasureProbability[1] = config.enemyAndTreasureProbability[0] + diffF;
            config.enemyAndTreasureProbability[2] = config.enemyAndTreasureProbability[1] + diffF;
            config.enemyAndTreasureProbability[3] = config.enemyAndTreasureProbability[2] + diffF;
            config.enemyAndTreasureProbability[4] = config.enemyAndTreasureProbability[3] + diffF;

            config.traderProbability = Scale(individual[individualIterator++], .01f, .1f);

            config.entryProbability = Scale(individual[individualIterator++], .01f, 1.0f);

            config.enemyProbabilityPrices[0] = 0;
            diffI = Scale(individual[individualIterator++], 5, 50);
            config.enemyProbabilityPrices[1] = config.enemyProbabilityPrices[0] + diffI;
            config.enemyProbabilityPrices[2] = config.enemyProbabilityPrices[1] + diffI;
            config.enemyProbabilityPrices[3] = config.enemyProbabilityPrices[2] + diffI;
            config.enemyProbabilityPrices[4] = config.enemyProbabilityPrices[3] + diffI;

            config.enemyLevelPrices[0] = 0;
            diffI = Scale(individual[individualIterator++], 5, 50);
            config.enemyLevelPrices[1] = config.enemyLevelPrices[0] + diffI;
            config.enemyLevelPrices[2] = config.enemyLevelPrices[1] + diffI;
            config.enemyLevelPrices[3] = config.enemyLevelPrices[2] + diffI;
            config.enemyLevelPrices[4] = config.enemyLevelPrices[3] + diffI;

            config.upgradePotionPrices[0] = 0;
            diffI = Scale(individual[individualIterator++], 5, 50);
            config.upgradePotionPrices[1] = config.upgradePotionPrices[0] + diffI;
            config.upgradePotionPrices[2] = config.upgradePotionPrices[1] + diffI;

            config.upgradeInitPotionsPrices[0] = 0;
            diffI = Scale(individual[individualIterator++], 5, 50);
            config.upgradeInitPotionsPrices[1] = config.upgradeInitPotionsPrices[0] + diffI;
            config.upgradeInitPotionsPrices[2] = config.upgradeInitPotionsPrices[1] + diffI;

            config.upgradeInitSpellsPrices[0] = 0;
            diffI = Scale(individual[individualIterator++], 5, 50);
            config.upgradeInitSpellsPrices[1] = config.upgradeInitSpellsPrices[0] + diffI;
            config.upgradeInitSpellsPrices[2] = config.upgradeInitSpellsPrices[1] + diffI;

            config.energyPrices[0] = 0;
            diffI = Scale(individual[individualIterator++], 5, 50);
            config.energyPrices[1] = config.energyPrices[0] + diffI;
            config.energyPrices[2] = config.energyPrices[1] + diffI;
            config.energyPrices[3] = config.energyPrices[2] + diffI;
            config.energyPrices[4] = config.energyPrices[3] + diffI;

            config.tokensCountForPrice = Scale(individual[individualIterator++], 1, 20);

            // playerDefaultHealthPoints // static
            // playerDefaultLives // static

            config.potionPrice = Scale(individual[individualIterator++], 1, 20);
            config.spellPrice = Scale(individual[individualIterator++], 1, 20);
            config.tokenPrice = config.tokensCountForPrice * 10; // static

            // enemyLevelRanges static

            config.enemyDropRateKeyPerLevel = Scale(individual[individualIterator++], .01f, .2f);
            config.enemyDropRateWeapon = Scale(individual[individualIterator++], .01f, .2f);
            config.enemyDropMoneyCountPerLevel = Scale(individual[individualIterator++], 1, 10);
            config.enemyDamageCountPerLevel = Scale(individual[individualIterator++], 1, 20);
            config.enemyHealthCountPerLevel = Scale(individual[individualIterator++], 2, 40);
            config.enemyHealthCountBase = Scale(individual[individualIterator++], 10, 50);

            config.weaponDamageDiceSides[0] = 5;
            diffI = Scale(individual[individualIterator++], 3, 20);
            config.weaponDamageDiceSides[1] = config.weaponDamageDiceSides[0] + diffI;
            config.weaponDamageDiceSides[2] = config.weaponDamageDiceSides[1] + diffI;
            config.weaponDamageDiceSides[3] = config.weaponDamageDiceSides[2] + diffI;

            // healthPotionRegeneration static

            config.spellDamageIncrease[0] = Scale(individual[individualIterator++], 5, 40);
            diffI = Scale(individual[individualIterator++], 10, 50);
            config.spellDamageIncrease[1] = config.weaponDamageDiceSides[0] + diffI;
            config.spellDamageIncrease[2] = config.weaponDamageDiceSides[1] + diffI;

            config.treasureDropMoneyCountMin = Scale(individual[individualIterator++], 5, 30);
            config.treasureDropMoneyCountMax = config.treasureDropMoneyCountMin + Scale(individual[individualIterator++], 5, 30);
            config.treasureDropTokensCountMin = Scale(individual[individualIterator++], 5, 30);
            config.treasureDropTokensCountMax = config.treasureDropTokensCountMin + Scale(individual[individualIterator++], 5, 30);

            return config;
        }

        [Pure]
        private static int Scale(int value, int min, int max)
        {
            float range = max - min;
            float adjustment = (value / (float)MAX_VAL) * range;
            float adjustedValue = min + adjustment;
            return (int)(adjustedValue);
        }

        [Pure]
        private static float Scale(int value, float min, float max)
        {
            float range = max - min;
            float adjustment = (value / (float)MAX_VAL) * range;
            float adjustedValue = min + adjustment;
            return (float)(adjustedValue);
        }
    }
}
