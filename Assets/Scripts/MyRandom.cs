using UnityEngine;

namespace Assets.Scripts
{
    static class MyRandom
    {
        public static int RangeInt(int seed, int min, int max)
        {
            Random.InitState(seed);
            return Random.Range(min, max);
        }
        public static double NextFloat(int seed)
        {
            Random.InitState(seed);
            return Random.value;
        }
    }
}
