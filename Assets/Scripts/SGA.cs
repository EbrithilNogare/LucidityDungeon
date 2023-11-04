using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    class SGA : MonoBehaviour
    {
        static readonly int POP_SIZE = 100;
        static readonly int IND_LEN = 80;
        static readonly int MAX_GEN = 100;
        static readonly int MAX_VAL = 100;
        static readonly double CX_PROB = 0.8;
        static readonly double MUT_PROB = 0.05;
        static readonly double MUT_FLIP_PROB = 0.1;
        static System.Random random = new System.Random();

        static List<int> CreateIndividual(int length)
        {
            List<int> individual = new List<int>();
            for (int i = 0; i < length; i++)
            {
                individual.Add(random.Next(MAX_VAL));
            }
            return individual;
        }

        static List<List<int>> CreatePopulation(int size)
        {
            List<List<int>> population = new List<List<int>>();
            for (int i = 0; i < size; i++)
            {
                population.Add(CreateIndividual(IND_LEN));
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
                if (fits[index1] > fits[index2])
                {
                    selected.Add(new List<int>(population[index1]));
                }
                else
                {
                    selected.Add(new List<int>(population[index2]));
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
            List<Tuple<int, int, double, int>> log = new List<Tuple<int, int, double, int>>();
            for (int G = 0; G < MAX_GEN; G++)
            {
                List<int> fits = population.Select(fitnessFunction).ToList();
                Debug.Log("Generation: " + G);
                Debug.Log("fits.Max(): " + fits.Max());
                Debug.Log("fits.Average(): " + fits.Average());
                Debug.Log(string.Join(", ", population[fits.IndexOf(fits.Max())]));
                List<List<int>> matingPool = Selection(population, fits);
                List<List<int>> offspring = MutationPopulation(CrossoverPopulation(matingPool));
                population = offspring;
            }
            return population;
        }

        static int FitnessFunction(List<int> individual)
        {
            // todo
            return individual.Sum();
        }

        public static void Main()
        {
            List<List<int>> population = CreatePopulation(POP_SIZE);

            population = EvolutionaryAlgorithm(population, FitnessFunction);
        }
    }
}
