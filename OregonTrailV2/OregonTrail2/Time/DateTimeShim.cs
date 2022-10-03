namespace OregonTrail2.Time
{
    public class DateTimeShim : ITimeService
    {
        public DateTime GetCurrentTime() => DateTime.UtcNow;
    }
}
