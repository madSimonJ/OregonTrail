using OregonTrail.Entities;

namespace OregonTrail.Turns
{
    internal class EventConfiguration
    {
        public int ProbabilityLimit { get; set; }
        public Event EventType { get; set; }
    }

    //    ,
    //    ,
    //    ,
    //    ,
    //    ,
    //    ,
    //    ,
    //    ,
    //    WildAnimalsAttack,
    //    HailStorm,
    //    Illness,
    //    HelpfulIndians,
    //    RuggedMountains,

    public class EndOfTurnEventHandler : IHandleEndOfTurnEvents
    {
        private readonly IEnumerable<EventConfiguration> Events = new[]
            {
                new EventConfiguration
                {
                    ProbabilityLimit = 6,
                    EventType = Event.WagonBreaksDown
                },
                new EventConfiguration
                {
                    ProbabilityLimit = 11,
                    EventType = Event.OxInjuresLeg
                },
                new EventConfiguration
                {
                    ProbabilityLimit = 13,
                    EventType = Event.DaughterBrokeHerArm
                },
                new EventConfiguration
                {
                    ProbabilityLimit = 15,
                    EventType = Event.OxWandersOff
                },
                new EventConfiguration
                {
                    ProbabilityLimit = 17,
                    EventType = Event.SonGetsLost
                },
                new EventConfiguration
                {
                    ProbabilityLimit = 22,
                    EventType = Event.UnsafeWater
                },
                new EventConfiguration
                {
                    ProbabilityLimit = 32,
                    EventType = Event.HeavyRain
                },
                new EventConfiguration
                {
                    ProbabilityLimit = 35,
                    EventType = Event.BanditsAttack
                },
                new EventConfiguration
                {
                    ProbabilityLimit = 37,
                    EventType = Event.FireInWagon
                },
                new EventConfiguration
                {
                    ProbabilityLimit = 42,
                    EventType = Event.LostInHeavyFog
                },
                new EventConfiguration
                {
                    ProbabilityLimit = 44,
                    EventType = Event.KilledPoisonousSnake
                },
                new EventConfiguration
                {
                    ProbabilityLimit = 54,
                    EventType = Event.WagonSwampedFordingRiver
                },
            };


        public EndOfTurnEventHandler()
        {
            
        }

        public GameState HandleEndOfTurn(GameState currentState, string userInput)
        {
            throw new NotImplementedException();
        }
    }
}
