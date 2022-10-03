using OregonTrail2.PlayerInteraction;
using OregonTrail2.Time;
using System;


namespace OregonTrail2.Shooting
{
    public  class GunShotClient : IShootTheGun
    {
        private readonly IPlayerInteraction playerInteraction;
        private readonly ITimeService timeService;

        public GunShotClient(IPlayerInteraction playerInteraction, ITimeService timeService)
        {
            this.playerInteraction = playerInteraction;
            this.timeService = timeService;
        }
        public int Shoot()
        {
            var startTime = this.timeService.GetCurrentTime();
            var input = this.playerInteraction.GetInput("TYPE BANG");
            var hit = input is TextInput ti && ti.TextFromUser.ToUpper() == "BANG";
            var timeTaken = hit ? (this.timeService.GetCurrentTime() - startTime).Seconds : 7;
            return timeTaken > 7
                        ? 7
                        : timeTaken;
        }
    }
}


