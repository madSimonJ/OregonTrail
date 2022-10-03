
namespace OregonTrail.TimeService
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrentTime() => DateTime.Now;
    }
}
