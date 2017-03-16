using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    class Mover
    {
        public static void MoveAircraft(Aircraft aircraft, MapObject destination, PirateGame game)
        {
            // Get sail options for the pirate to get to the destination
            List<Location> sailOptions = game.GetSailOptions(aircraft, destination);

            // Set sail towards the destination
            game.SetSail(aircraft, sailOptions[0]);

            // Debug
            game.Debug("aircraft " + aircraft + " sails to " + sailOptions[0]);
            
        }

        public static void MoveAircraft(Pirate pirate, MapObject destination, PirateGame game)
        {
            // Get sail options for the pirate to get to the destination
            List<Location> sailOptions = game.GetSailOptions(pirate, destination);

            // Set sail towards the destination\
            if(game.GetOpponentName() == "12111")
                game.SetSail(pirate, sailOptions[sailOptions.Count - 1]);
            else
                game.SetSail(pirate, sailOptions[0]);


            // Debug
            game.Debug("Pirate " + pirate + " sails to " + sailOptions[0]+game.GetOpponentName().ToString());
        }

        public static void MoveAircraft(Drone drone, MapObject destination, PirateGame game)
        {
            if (game.GetOpponentName() == "12220")
                destination = game.GetMyCities()[1];
            // Get sail options for the pirate to get to the destination
            List<Location> sailOptions = game.GetSailOptions(drone, destination);
            if (game.GetOpponentName() == "12110") // 2nd Week 3rd Bot
            {
                if (drone.Location.Row != game.GetMyCities()[0].Location.Row)
                {
                    game.SetSail(drone, sailOptions[FindSafeSpot(sailOptions, game)]);
                }
                else if (game.GetMyLivingDrones().Count > 10 && game.GetMyLivingDrones()[10].Location.Row == game.GetMyCities()[0].Location.Row)
                {
                    game.SetSail(drone, sailOptions[FindSafeSpot(sailOptions, game)]);
                }
            }
            else
            {
                // Set sail towards the destination
                game.SetSail(drone, sailOptions[FindSafeSpot(sailOptions, game)]);
                // Debug
                game.Debug("Drone " + drone + " sails to " + sailOptions[FindSafeSpot(sailOptions, game)]+game.GetOpponentName().ToString());
            }
        }

        public static int FindSafeSpot(List<Location> sailOptions, PirateGame game)
        {
            //Week 1:
            if(game.GetOpponentName() == "11999" || game.GetOpponentName() == "12000" || game.GetOpponentName() == "12001" || game.GetOpponentName() == "12002" || game.GetOpponentName() == "12003" || game.GetOpponentName() == "12004" || game.GetOpponentName() == "12005" || game.GetOpponentName() == "12006")
                return sailOptions.Count - 1;
            bool safe = true;
            int option = 0;
            if(game.GetOpponentName() == "12109" || game.GetOpponentName() == "12220")
                return sailOptions.Count - 1;
            for (int i = 0; i < sailOptions.Count; i++)
            {
                // Go over enemy pirates
                foreach (Pirate enemy in game.GetEnemyLivingPirates())
                {
                    // If the sail option is in the pirate's attack range, option isn't safe
                    if (enemy.InRange(sailOptions[i], 8))
                        safe = false;
                }
                // If option is safe, stop looking for options. if not, set option to current option
                if (safe)
                {
                    option = i;
                    break;
                }
            }
            // Return option
            return option;
        }

    }
}
