namespace OregonTrail.Random
{
    public class RandomNumberGenerator : IGenerateRandomNumbers
    {
        public bool AreRidersAhead(int milesTravelled)
        {
            var ridersAheadLimit = (milesTravelled / 100 - 4) ^ 2 + 72 / ((milesTravelled / 100 - 4) ^ 2 + 12) - 1;
            return BetweenZeroAnd(10) < ridersAheadLimit;
        }

        public bool AreRidersFriendly()
        {
            return BetweenZeroAnd(10) < 8;
        }

        public bool AreRidersActuallyFriendly(bool seemFriendly)
        {
            return BetweenZeroAnd(10) < 2 ? !seemFriendly : seemFriendly;
        }

        public bool DoRidersStillAttack()
        {
            var rnd = BetweenZeroAnd(10);
            return rnd <= 8;
        }

        public int BetweenZeroAnd(int to)
        {
            var rnd = new System.Random(DateTime.Now.Millisecond);
            return rnd.Next(0, to);
        }
    }
}
