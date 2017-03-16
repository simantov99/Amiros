using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class Targets
    {
        public static Location GetTargetPirate(Pirate pirate, PirateGame game)
        {
            try { return game.GetNotMyIslands().OrderBy(c => c.Distance(pirate)).ToList()[0].Location; }
            catch { return game.GetEnemyLivingPirates().OrderBy(p => p.Distance(pirate)).ToList()[0].Location; }

        }
        
        public static Location GetTarget<T>(Pirate pirate, PirateGame game, int range)
        {
            return GetTarget<T>(pirate, game, pirate, range);
        }
        
        public static Location GetTarget<T>(Pirate pirate, PirateGame game, MapObject Obj, int range)
        {
            if (game.GetEnemyLivingDrones().Count > 0)
            {
                if (TryGetTargetList(Obj, game, range))
                {
                    List<Aircraft> targets = GetTargetList(Obj, game, range);
                    if (targets.Where(c => c.GetType().Equals(typeof(T))).ToList().Count > 0)
                        return targets.Where(c => c.GetType().Equals(typeof(T))).ToList()[0].Location;
                }
            }
            return game.GetNotMyCities().OrderBy(c => c.Distance(Obj)).ToList()[0].Location;
        }

        public static List<Aircraft> GetTargetList(MapObject Obj, PirateGame game, int range)
        {
            return game.GetEnemyLivingAircrafts().Where(d => game.GetEnemyCities().OrderBy(c => c.Distance(Obj)).ToList()[0].InRange(d, range)).ToList();
        }

        public static bool TryGetTargetList(MapObject Obj, PirateGame game, int range)
        {
            try { GetTargetList(Obj, game, range); }
            catch { return false; }
            return true;
        }

    }
}
