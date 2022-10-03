namespace OregonTrail.Common
{
    public static class FunctionalExtensions
    {
        public static TOutput Map<TInput, TOutput>(this TInput @this, Func<TInput, TOutput> f)
        {
            return f(@this);
        }


        public static T IterateUntil<T>(
            this T @this,
            Func<T, T> createNext,
            Func<T, bool> finishCondition)
        {
            var isFinished = finishCondition(@this);
            if (isFinished)
            {
                return @this;
            }
            else
            {
                return IterateUntil(
                        createNext(@this),
                        createNext,
                        finishCondition);
                    
            }
        }
    }
}
