using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class Week1
    {
        public static Drone GetClosestDrone(PirateGame game)
        {

            List<Drone> enemydrones = game.GetEnemyLivingDrones();
            if (enemydrones.Count > 0)
            {
                int min = 100;
                Drone dro = enemydrones[0];
                foreach (Drone d in enemydrones)
                {
                    int dis = d.Distance(game.GetEnemyCities()[0]);
                    if (dis < min)
                    {
                        min = dis;
                        dro = d;
                    }
                }
                return dro;
            }
            return null;
        }

        public static Location GetGuardDestination(Pirate pirate, PirateGame game)
        {
            Drone drone = GetClosestDrone(game);
            if (drone != null)
            {
                if (drone.Location.Col > game.GetEnemyCities()[0].Location.Col + 3 && drone.Location.Row <= game.GetEnemyCities()[0].Location.Row + 3 && drone.Location.Row >= game.GetEnemyCities()[0].Location.Row - 3)
                {
                    return new Location(game.GetEnemyCities()[0].Location.Row, game.GetEnemyCities()[0].Location.Col + 1);
                }
                else if (drone.Location.Col < game.GetEnemyCities()[0].Location.Col - 3 && drone.Location.Row <= game.GetEnemyCities()[0].Location.Row + 3 && drone.Location.Row >= game.GetEnemyCities()[0].Location.Row - 3)
                {
                    return new Location(game.GetEnemyCities()[0].Location.Row, game.GetEnemyCities()[0].Location.Col - 1);
                }
                else if (drone.Location.Row > game.GetEnemyCities()[0].Location.Row + 1)
                {
                    return new Location(game.GetEnemyCities()[0].Location.Row + 1, game.GetEnemyCities()[0].Location.Col);
                }
                else if (drone.Location.Row < game.GetEnemyCities()[0].Location.Row - 1)
                {
                    return new Location(game.GetEnemyCities()[0].Location.Row - 1, game.GetEnemyCities()[0].Location.Col);
                }
            }
            return game.GetEnemyCities()[0].Location;

        }

        public static bool GoAsUnit(PirateGame game)
        {
            bool startgo = false;
            List<Drone> livingdrones = game.GetMyLivingDrones();

            if (livingdrones != null)
            {
                if (livingdrones.Count >= game.GetMaxDronesCount())
                {
                    startgo = true;
                }
            }
            return startgo;
        }
    }
}
