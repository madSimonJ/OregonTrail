namespace OregonTrail2.Common
{
    public abstract class Maybe<T>
    {

    }

    public class Nothing<T> : Maybe<T>
    {

    }

    public class Something<T> : Maybe<T>
    {
        public Something(T value)
        {
            this.Value = value;
        }

        public T Value { get; set; }
    }

    public class Error<T> : Maybe<T>
    {
        public Error(Exception capturedError)
        {
            CapturedError = capturedError;
        }
        public Exception CapturedError { get; set; }
    }

    public static class MaybeExtensions
    {
        public static Maybe<T> ToMaybe<T>(this T @this) =>
            new Something<T>(@this);
    }
}
