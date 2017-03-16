class Functions
    {
        
        public static void SendAirCraftToLocation(PirateGame game , Aircraft Ac , Location L)
        {
            List<Location> _sailOptions = game.GetSailOptions(Ac,L);
            game.SetSail(Ac,_sailOptions[0]);
        }
        public Drone ClosesEnemytDroneToMapObject(PirateGame game, MapObject L)
        {
            try
            {
                return game.GetEnemyLivingDrones().OrderBy(Drone => Drone.Distance(L)).ToList()[0];
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
