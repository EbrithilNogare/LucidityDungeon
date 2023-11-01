using System;

namespace Assets.Scripts
{
    internal static class MyRandom
    {
        public static int RangeInt(int seed, int min, int max)
        {
            Random random = new Random(seed);
            // warm up
            random.Next();
            random.Next();
            random.Next();
            return random.Next(min, max);
        }
        public static int NextInt(int seed)
        {
            Random random = new Random(seed);
            // warm up
            random.Next();
            random.Next();
            random.Next();
            return random.Next();
        }
        public static double NextDouble(int seed)
        {
            Random random = new Random(seed);
            // warm up
            random.Next();
            random.Next();
            random.Next();
            return random.NextDouble();
        }
    }
}
