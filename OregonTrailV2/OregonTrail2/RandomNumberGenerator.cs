namespace OregonTrail2
{
    public class RandomNumberGenerator : IRandomNumberGenerator
    {
        public int BetweenZeroAnd(int input) =>
            new Random(new DateTime().Millisecond).Next(0, input);
    }
}
