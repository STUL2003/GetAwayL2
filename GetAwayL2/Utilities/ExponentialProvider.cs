
namespace GetAwayL2.Utilities
{
    static public class ExponentialProvider
    {
        private static readonly Random random = new Random();
        private static readonly object randomLock = new object();

        public static TimeSpan Calculate(int attemptNumber)
        {
            int jitter = 0;
            lock (randomLock) //because Random is not threadsafe
                jitter = random.Next(10, 200);

            return TimeSpan.FromSeconds(Math.Pow(2, attemptNumber - 1)) + TimeSpan.FromMilliseconds(jitter);
        }
    }
}
