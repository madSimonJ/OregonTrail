namespace OregonTrail.Random
{
    public interface IGenerateRandomNumbers
    {
        int BetweenZeroAnd(int to);
        bool DoRidersStillAttack();
        bool AreRidersAhead(int milesTravelled);
        bool AreRidersFriendly();
        bool AreRidersActuallyFriendly(bool seemFriendly);
    }
}
