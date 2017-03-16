class Functions
    {
        

        public Drone ClosesEnemytDroneToMApObject(PirateGame game, MapObject L)
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
        public List<Aircraft> AirCraftInRange(PirateGame game , Aircraft Ac , int Radius)
        {
            try
            {
                return game.GetEnemyLivingAircrafts().Where(Aircraft => Ac.InRange(Aircraft, Radius)).ToList();
            }
            catch (Exception)
            {

                return null;
            }
        }
        
    }
