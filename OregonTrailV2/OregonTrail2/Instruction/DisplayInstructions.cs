using OregonTrail2.PlayerInteraction;

namespace OregonTrail2.Instruction
{
    public class DisplayInstructions : IDisplayInstructions
    {
        private readonly IPlayerInteraction userInteraction;

        public DisplayInstructions(IPlayerInteraction userInteraction)
        {
            this.userInteraction = userInteraction;
        }

        void IDisplayInstructions.DisplayInstructions()
        {
            var displayInstructionsAnswer = this.userInteraction.GetInput("DO YOU NEED INSTRUCTIONS  (YES/NO)");
            var displayInstructions = displayInstructionsAnswer switch
            {
                TextInput ti when ti.TextFromUser.ToUpper() == "YES" => true,
                _ => false
            };

            this.userInteraction.WriteMessageConditional(displayInstructions,
                    string.Empty,
                    string.Empty,
                    "THIS PROGRAM SIMULATES A TRIP OVER THE OREGON TRAIL FROM",
                    "INDEPENDENCE, MISSOURI TO OREGON CITY, OREGON IN 1847.",
                    "YOUR FAMILY OF FIVE WILL COVER THE 2000 MILE OREGON TRAIL",
                    "IN 5-6 MONTHS --- IF YOU MAKE IT ALIVE.",
                    string.Empty,
                    "YOU HAD SAVED $900 TO SPEND FOR THE TRIP, AND YOU'VE JUST",
                    "   PAID $200 FOR A WAGON.",
                    "YOU WILL NEED TO SPEND THE REST OF YOUR MONEY ON THE",
                    "   FOLLOWING ITEMS:",
                    string.Empty,
                    "     OXEN - YOU CAN SPEND $200-$300 ON YOUR TEAM",
                    "            THE MORE YOU SPEND, THE FASTER YOU'LL GO",
                    "               BECAUSE YOU'LL HAVE BETTER ANIMALS",
                    string.Empty,
                    "     FOOD - THE MORE YOU HAVE, THE LESS CHANCE THERE",
                    "               IS OF GETTING SICK",
                    string.Empty,
                    "     AMMUNITION - $1 BUYS A BELT OF 50 BULLETS",
                    "            YOU WILL NEED BULLETS FOR ATTACKS BY ANIMALS",
                    "               AND BANDITS, AND FOR HUNTING FOOD",
                    string.Empty,
                    "     CLOTHING - THIS IS ESPECIALLY IMPORTANT FOR THE COLD",
                    "               WEATHER YOU WILL ENCOUNTER WHEN CROSSING",
                    "               THE MOUNTAINS",
                    string.Empty,
                    "     MISCELLANEOUS SUPPLIES - THIS INCLUDES MEDICINE AND",
                    "               OTHER THINGS YOU WILL NEED FOR SICKNESS",
                    "               AND EMERGENCY REPAIRS",
                    string.Empty,
                    string.Empty,
                    "YOU CAN SPEND ALL YOUR MONEY BEFORE YOU START YOUR TRIP -",
                    "OR YOU CAN SAVE SOME OF YOUR CASH TO SPEND AT FORTS ALONG",
                    "THE WAY WHEN YOU RUN LOW.  HOWEVER, ITEMS COST MORE AT",
                    "THE FORTS.  YOU CAN ALSO GO HUNTING ALONG THE WAY TO GET",
                    "MORE FOOD.",
                    "WHENEVER YOU HAVE TO USE YOUR TRUSTY RIFLE ALONG THE WAY,",
                    "YOU WILL SEE THE WORDS: TYPE BANG.  THE FASTER YOU TYPE",
                    "IN THE WORD 'BANG' AND HIT THE 'RETURN' KEY, THE BETTER",
                    "LUCK YOU'LL HAVE WITH YOUR GUN.",
                    string.Empty,
                    "WHEN ASKED TO ENTER MONEY AMOUNTS, DON'T USE A '$'.",
                    "GOOD LUCK!!!",
                    string.Empty,
                    string.Empty
                );
        }
    }
}
