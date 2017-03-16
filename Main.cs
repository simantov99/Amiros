using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class Main : IPirateBot
    {
        bool done = false; //Got island 0
        public void DoTurn(PirateGame game)
        {
            game.Debug(game.GetOpponentName());
            // Give orders to my pirates
            HandlePirates(game);
            // Give orders to my drones if my city exists
            if (game.GetMyCities().Count > 0 || game.GetNeutralCities().Count > 0)
                HandleDrones(game);
                
            HandleDecoy(game);
        }

        private void HandleDecoy(PirateGame game)
        {
            if (game.GetMyDecoy() != null)
            {
                if(game.GetMyLivingPirates().Count > 1)
                    Mover.MoveAircraft(game.GetMyself().Decoy, game.GetMyLivingPirates().OrderBy(p=>p.Distance(game.GetMyself().Decoy)).ToList()[1], game);
                else if (game.GetMyLivingPirates().Count > 0)
                    Mover.MoveAircraft(game.GetMyself().Decoy, game.GetMyLivingPirates().OrderBy(p=>p.Distance(game.GetMyself().Decoy)).ToList()[0], game);
                else
                    Mover.MoveAircraft(game.GetMyself().Decoy, game.GetAllIslands()[0], game);
            }
        }
        
        private void HandlePirates(PirateGame game)
        {
            bool second_guard = false;
            bool decoyed = false;
            // Go over all of my pirates
            foreach (Pirate pirate in game.GetMyLivingPirates())
            {
                #region First Week
                if(game.GetOpponentName() == "11999" || game.GetOpponentName() == "12000" || game.GetOpponentName() == "12001" || game.GetOpponentName() == "12002" || game.GetOpponentName() == "12003" || game.GetOpponentName() == "12004" || game.GetOpponentName() == "12005" || game.GetOpponentName() == "12006")
                {
                    if (!Attacker.TryAttack(pirate, game))
                    {
                        //bot 1:
                        if (game.GetMyCities().Count == 0)
                            Mover.MoveAircraft(pirate, Week1.GetGuardDestination(pirate, game), game);
                        else
                        {
                            // if there's 5 islands OR my city and first island are on the same row and there is more than one island and not one city
                            if (game.GetAllIslands().Count == 3 || game.GetMyCities()[0].Location.Row != game.GetAllIslands()[0].Location.Row && (game.GetMyCities().Count != 1 && game.GetAllIslands().Count != 1))
                            {
                                // if there's 3 islands AND my city and enemy city is in the same row
                                if (game.GetAllIslands().Count == 3 && game.GetMyCities()[0].Location.Row == game.GetEnemyCities()[0].Location.Row)
                                {
                                    if (pirate.Id == 0)
                                    {
                                        Mover.MoveAircraft(pirate, Week1.GetGuardDestination(pirate, game), game);
                                    }
                                    else if (pirate.Id < 3)
                                    {
                                        int distance = 1000000;
                                        int id = 0;
                                        int counter = 0;
                                        foreach (Aircraft a in game.GetEnemyLivingAircrafts())
                                        {
                                            if (a.Distance(pirate) < distance)
                                            {
                                                id = counter;
                                                distance = a.Distance(pirate);
                                            }
                                            counter++;
                                        }
                                        // sail to closest enemy aircraft
                                        Mover.MoveAircraft(pirate, game.GetEnemyLivingAircrafts()[id], game);
                                    }
                                    else if (pirate.Id == 3)
                                        Mover.MoveAircraft(pirate, game.GetAllIslands()[0], game);
                                    else
                                        Mover.MoveAircraft(pirate, game.GetAllIslands()[1], game);
                                }
                                else //Dolphin OR Nahshol
                                    Mover.MoveAircraft(pirate, game.GetAllIslands()[2], game);
                            }
                            else if (!Week1.GoAsUnit(game) && !done)
                            {
                                // Gal before move
                                Mover.MoveAircraft(pirate, game.GetAllIslands()[0], game);
                            }
                            else if (game.GetMyCities().Count == 1 && game.GetAllIslands().Count == 1 && game.GetMyCities()[0].Location.Col == game.GetAllIslands()[0].Location.Col)
                            {
                                // Gal after move
                                done = true;
                                Mover.MoveAircraft(pirate, game.GetMyLivingDrones()[0], game);
                            }
                            else if (game.GetMyCities()[0].Location.Row == game.GetAllIslands()[0].Location.Row)
                            {
                                // Bee
                                done = true;
                                Mover.MoveAircraft(pirate, game.GetEnemyLivingPirates()[0], game);
                            }
                        }
                    }
                }
                #endregion
                
                #region Second Week
                else if(game.GetOpponentName() == "12109") // First Bot
                {
                    if (pirate.Id == 0) // If You're The First Pirate
                        {
                            if (game.GetNotMyIslands().Count > 0)
                            {
                                if (game.GetNeutralCities().Count > 0 && game.GetAllIslands().OrderBy(c => c.Distance(game.GetNeutralCities()[0])).ToList()[0].Owner != game.GetMyself()) // If There's a trading city and the island closest to it isn't ours
                                    Mover.MoveAircraft(pirate, game.GetNotMyIslands().OrderBy(c => c.Distance(game.GetNeutralCities()[0])).ToList()[0], game); // Go To Closest Island To The Trading City
                                else
                                    Mover.MoveAircraft(pirate, game.GetNotMyIslands().OrderBy(c => c.Distance(pirate)).ToList()[0], game); // Go To Closest Island That isn't Yours
                            }
                            else
                                Mover.MoveAircraft(pirate, game.GetMyIslands().OrderByDescending(c => c.Distance(pirate)).ToList()[0], game); // Go To Farthest Island That's Yours
                        }
                        // If pirate didn't attack
                        else if (!Attacker.TryAttack(pirate, game))
                        {
                            foreach (City C in game.GetEnemyCities())
                                if (pirate.Id == game.GetMyLivingPirates().OrderBy(c => c.Distance(C)).ToList()[0].Id) // If current pirate is the pirate that is closest to the current enemy city
                                    Mover.MoveAircraft(pirate, Targets.GetTarget<Drone>(pirate, game, C, 10), game);// Guard In Range Of 10
                                else if (game.GetNotMyIslands().Count > 0) // if there are islands that aren't ours
                                    Mover.MoveAircraft(pirate, game.GetNotMyIslands().OrderBy(c => c.Distance(pirate)).ToList()[0], game); // Go To Closest Island That Isn't Yours
                                else
                                    Mover.MoveAircraft(pirate, game.GetMyIslands().OrderByDescending(c => c.Distance(pirate)).ToList()[0], game); // Go To Farthest Island That's Yours
                        }
                }
                else if (game.GetOpponentName() == "12116") // Last Bot
                {
                    if(!Attacker.TryAttack(pirate, game)) // if pirate didn't attack
                        Mover.MoveAircraft(pirate, game.GetAllIslands().OrderBy(isle => isle.Distance(game.GetNeutralCities()[0])).ToList()[0], game); // go to the closest islnad to the trade city
                }
                else if (game.GetOpponentName() == "12115") // Seventh Bot
                {
                    if (!Attacker.TryAttack(pirate, game))
                        {
                            //if pirate is the closest to the trade city(optimal guard), guard the trade city
                            if (pirate == game.GetMyLivingPirates().OrderBy(c => c.Distance(game.GetNeutralCities()[0])).ToList()[0])
                                Mover.MoveAircraft(pirate, Targets.GetTarget<Drone>(pirate, game, game.GetNeutralCities().OrderBy(c => c.Distance(pirate)).ToList()[0], 15), game);// Guard In Range Of 15
                            //find if one of the other enemy cities has drones in its range, go there too
                            //if we don't own the closest island to trade city, go get 'em
                            else if (game.GetAllIslands().OrderBy(c => c.Distance(game.GetNeutralCities()[0])).ToList()[0].Owner != game.GetMyself())
                                Mover.MoveAircraft(pirate, game.GetAllIslands().OrderBy(c => c.Distance(game.GetNeutralCities()[0])).ToList()[0].Location, game);
                            //if trade city is under attack, go help.
                            else if (game.GetEnemyLivingPirates().Exists(e => e.InRange(game.GetMyLivingPirates().OrderBy(c => c.Distance(game.GetNeutralCities()[0])).ToList()[0], 7)))
                            {
                                try
                                {
                                    if (pirate.Id < game.GetEnemyLivingPirates().Count(e => e.InRange(game.GetMyLivingPirates().OrderBy(c => c.Distance(game.GetNeutralCities()[0])).ToList()[0], 7)))
                                        Mover.MoveAircraft(pirate, Targets.GetTarget<Pirate>(pirate, game, game.GetNeutralCities().OrderBy(c => c.Distance(pirate)).ToList()[0], 10), game);
                                    else
                                        Mover.MoveAircraft(pirate, Targets.GetTarget<Pirate>(pirate, game, game.GetNotMyIslands().OrderBy(c => c.Distance(pirate)).ToList()[0], 15), game);
                                }
                                catch { }
    
                            }
                            else if (!second_guard && game.GetEnemyLivingDrones().Exists(d => d.InRange(game.GetEnemyCities().OrderBy(c => c.Distance(pirate)).ToList()[0], 10)))
                            {
                                Mover.MoveAircraft(pirate, Targets.GetTarget<Drone>(pirate, game, game.GetEnemyCities().OrderBy(c => c.Distance(pirate)).ToList()[0], 10), game);
                                second_guard = true;
    
                            }
                            //finally focus on capturing islands
                            else
                                try
                                {
                                    Mover.MoveAircraft(pirate, game.GetNotMyIslands().OrderBy(c => c.Distance(game.GetNeutralCities()[0])).ToList()[0].Location, game);
                                }
                                catch { }
                        }
                }
                else if (game.GetOpponentName() == "12110") // Second Bot
                {
                    if (!Attacker.TryAttack(pirate, game)) // if pirate didn't attack
                        Mover.MoveAircraft(pirate, game.GetEnemyLivingPirates().OrderBy(p=>p.Distance(pirate)).ToList()[0], game); // go to closest enemy pirate
                }
                else if (game.GetOpponentName() == "12111") // Third Bot
                {
                    if(!Attacker.TryAttack(pirate, game)) // if pirate didn't attack
                    {
                        if(game.GetEnemyLivingDrones().Count > 0) // if there's any enemy drones
                            Mover.MoveAircraft(pirate, game.GetEnemyLivingDrones().OrderBy(p=>p.Distance(game.GetEnemyLivingDrones()[0])).ToList()[0], game); // go to closest enemy drone
                        else
                            Mover.MoveAircraft(pirate, game.GetNeutralCities()[0], game); // go to trade city
                    }   
                }
                else if (game.GetOpponentName() == "12113") // Fifth Bot
                {
                    if(!Attacker.TryAttack(pirate, game)) // if pirate didn't attack
                    {
                        if (game.GetEnemyLivingDrones().Count > 0 && pirate.Id < 4) // if there are any enemy drones and the pirate id is less than 4
                            Mover.MoveAircraft(pirate, game.GetEnemyLivingDrones().OrderBy(d => d.Distance(game.GetNeutralCities()[0])).ToList()[0], game); // go to closest enemy drone to trade city
                        else if(game.GetNotMyIslands().Count > 0) // if there are any islands that aren't ours
                            Mover.MoveAircraft(pirate, game.GetNotMyIslands().OrderBy(p=>p.Distance(pirate)).ToList()[0], game); // go to closest island that isn't yours
                        else
                            Mover.MoveAircraft(pirate, game.GetEnemyLivingPirates().OrderBy(p=>p.Distance(pirate)).ToList()[0], game); // go to closest pirate
                    }
                }
                else if (game.GetOpponentName() == "12112") // Fourth Bot
                {
                    if (!Attacker.TryAttack(pirate, game)) // if pirate didn't attack
                    {
                        if(game.GetMyScore() > game.GetEnemyScore() + 5) // if our score is 6 more than the enemy's score
                        {
                            Mover.MoveAircraft(pirate, game.GetEnemyLivingPirates().OrderBy(c=>c.Distance(pirate)).ToList()[0], game); // go to dclosest enemy pirate
                        }
                        else if(pirate.Id == 0) // if pirate id is 0
                        {
                            if (game.GetEnemyLivingDrones().Count > 0) // if there are enemy drones
                                Mover.MoveAircraft(pirate, game.GetEnemyLivingDrones().OrderBy(c=>c.Distance(new Location(game.GetRowCount() / 2, game.GetColCount() / 2))).ToList()[0], game); // go to the closest enemy drone to the middle of the map
                            else
                                Mover.MoveAircraft(pirate, new Location(game.GetRowCount() / 2, game.GetColCount() / 2), game); // go to the middle of the map
                        }
                        else if(game.GetNotMyIslands().Count > 1) // if there's 2 or more islands that aren't ours
                            Mover.MoveAircraft(pirate, game.GetNotMyIslands()[pirate.Id % 2], game); // go to island 0 or 1 according to pirate id
                        else
                            Mover.MoveAircraft(pirate, game.GetEnemyLivingPirates()[0], game); // go to first enemy pirate
                    }
                }
                else if (game.GetOpponentName() == "12114") // 100 years of loneliness
                {
                    if(!Attacker.TryAttack(pirate, game))
                    {
                        if(pirate.Id == 0 && game.GetEnemyLivingDrones().Count > 0)
                            Mover.MoveAircraft(pirate, game.GetEnemyLivingDrones()[0], game);
                        else
                            Mover.MoveAircraft(pirate, game.GetAllIslands()[0], game); // go to first island
                    }
                }
                #endregion
                
                #region Third Week
                else if (game.GetOpponentName() == "12217") //First Bot
                {
                    // If pirate didn't attack move to the spot between the islands
                    if(!Attacker.TryAttack(pirate, game, game.GetEnemyLivingPirates().OrderBy(p=>p.Distance(pirate)).ToList()[0]))
                        Mover.MoveAircraft(pirate, new Location(14, 25), game);
                }
                else if (game.GetOpponentName() == "12218") // Second Bot
                {
                    // If pirate didn't attack go to the first island
                    if(!Attacker.TryAttack(pirate, game))
                        Mover.MoveAircraft(pirate, game.GetAllIslands()[0], game);
                }
                else if (game.GetOpponentName() == "12219") // Third Bot
                {
                    //If pirate id is either 0 or 7
                    if(pirate.Id == 0 || pirate.Id == 7)
                    {
                        // If pirate doesn't have a paintball get closest paintball
                        if (!pirate.HasPaintball)
                            Mover.MoveAircraft(pirate, game.GetAllPaintballs().OrderBy(p=>p.Distance(pirate)).ToList()[0], game);
                        // Else if there's any enemy drones
                        else if (game.GetEnemyLivingDrones().Count > 0)
                        {
                            // If pirate has an enemy drone in range attack it. If not, go to closest drone
                            if(!pirate.InAttackRange(game.GetEnemyLivingDrones().OrderBy(d=>d.Distance(pirate)).ToList()[0]))
                                Mover.MoveAircraft(pirate, game.GetEnemyLivingDrones().OrderBy(d=>d.Distance(pirate)).ToList()[0], game);
                            else
                                game.Attack(pirate, game.GetEnemyLivingDrones().OrderBy(d=>d.Distance(pirate)).ToList()[0]);
                        }
                        // Else go to closest island that isn't ours
                        else
                            Mover.MoveAircraft(pirate, game.GetNotMyIslands().OrderBy(p=>p.Distance(pirate)).ToList()[0], game);
                    }
                    // Else if pirate didn't attack
                    else if(!Attacker.TryAttack(pirate, game))
                    {
                        // If pirate isn't one of the last 3 pirates move to Second island
                        if(pirate.Id < game.GetAllMyPirates().Count - 2)
                            Mover.MoveAircraft(pirate, game.GetAllIslands()[2], game);
                        // Else if second island is ours target closest pirate to current pirate in a range of 30
                        else if (game.GetAllIslands()[2].Owner == game.GetMyself())
                            Mover.MoveAircraft(pirate, Targets.GetTarget<Pirate>(pirate, game, 24), game);
                        // Else go to second island
                        else
                            Mover.MoveAircraft(pirate, game.GetAllIslands()[2], game);
                            
                    }
                }
                else if (game.GetOpponentName() == "12220") // Fourth Bot
                {
                    // If pirate didn't attack
                    if(!Attacker.TryAttack(pirate, game))
                    {
                        // If pirate id is 0, go to (4,3). If not go staright untill you reach column 3
                        if(pirate.Id == 0)
                            Mover.MoveAircraft(pirate, new Location (4, 3), game);
                        else
                            Mover.MoveAircraft(pirate, new Location (pirate.Location.Row, 3), game);
                    }
                }
                else if (game.GetOpponentName() == "12221")
                {
                    // If pirate id is 0
                    if (pirate.Id == 0)
                    {
                        // If pirate doesn't have a paintball get closest paintball
                        if (!pirate.HasPaintball)
                            Mover.MoveAircraft(pirate, game.GetAllPaintballs().OrderBy(p=>p.Distance(pirate)).ToList()[0], game);
                        // Else if there's any enemy drones
                        else if (game.GetEnemyLivingDrones().Count > 0)
                        {
                            // If pirate has an enemy drone in range attack it. If not, go to closest drone
                            if(!pirate.InAttackRange(game.GetEnemyLivingDrones().OrderBy(d=>d.Distance(pirate)).ToList()[0]))
                                Mover.MoveAircraft(pirate, game.GetEnemyLivingDrones().OrderBy(d=>d.Distance(pirate)).ToList()[0], game);
                            else
                                game.Attack(pirate, game.GetEnemyLivingDrones().OrderBy(d=>d.Distance(pirate)).ToList()[0]);
                        }
                        // Else go to closest island that isn't ours
                        else
                            Mover.MoveAircraft(pirate, game.GetNotMyIslands().OrderBy(p=>p.Distance(pirate)).ToList()[0], game);
                    }
                    // Else if pirate didn't attack go to the first island
                    else if(!Attacker.TryAttack(pirate, game))
                        Mover.MoveAircraft(pirate, game.GetAllIslands()[0], game);
                }
                else if (game.GetOpponentName() == "12222")
                {
                    // If pirate didn't attack
                    if(!Attacker.TryAttack(pirate, game))
                    {
                        if (game.GetNotMyIslands().Count > 0)
                            Mover.MoveAircraft(pirate, game.GetNotMyIslands().OrderBy(i=>i.Distance(pirate)).ToList()[0], game);
                        else    
                            Mover.MoveAircraft(pirate, game.GetEnemyLivingPirates().OrderBy(p=>p.Distance(pirate)).ToList()[0], game);
                    }
                }
                else if (game.GetOpponentName() == "12223")
                {
                    if (game.GetNotMyIslands().Count > 0)
                        Mover.MoveAircraft(pirate, game.GetNotMyIslands().OrderBy(i=>i.Distance(pirate)).ToList()[0], game);
                    else if (pirate.Id < game.GetAllMyPirates().Count / 2) 
                    {
                        // If pirate doesn't have a paintball get closest paintball
                        if (!pirate.HasPaintball)
                            Mover.MoveAircraft(pirate, game.GetAllPaintballs().OrderBy(p=>p.Distance(pirate)).ToList()[0], game);
                        // Else if there's any enemy drones
                        else if (game.GetEnemyLivingDrones().Count > 0)
                        {
                            // If pirate has an enemy drone in range attack it. If not, go to closest drone
                            if(!pirate.InAttackRange(game.GetEnemyLivingDrones().OrderBy(d=>d.Distance(pirate)).ToList()[0]))
                                Mover.MoveAircraft(pirate, game.GetEnemyLivingDrones().OrderBy(d=>d.Distance(pirate)).ToList()[0], game);
                            else
                                game.Attack(pirate, game.GetEnemyLivingDrones().OrderBy(d=>d.Distance(pirate)).ToList()[0]);
                        }
                        // Else go to closest island that isn't ours
                        else if (!Attacker.TryAttack(pirate, game))
                            Mover.MoveAircraft(pirate, game.GetEnemyLivingPirates().OrderBy(p=>p.Distance(pirate)).ToList()[0], game);   
                    }
                    else if (!Attacker.TryAttack(pirate, game))
                    {
                        Mover.MoveAircraft(pirate, game.GetEnemyLivingPirates().OrderBy(p=>p.Distance(pirate)).ToList()[0], game);   
                    }
                }
                else if (false && game.GetOpponentName() == "12224")
                {
                    if (!Attacker.TryAttack(pirate, game))
                    {
                        if (pirate.Id < 4)
                        {
                            Mover.MoveAircraft(pirate, game.GetAllIslands()[3], game);
                        }
                        else
                        {
                            if (game.GetAllIslands()[1].Owner != game.GetMyself())
                                Mover.MoveAircraft(pirate, game.GetAllIslands()[1], game);
                            else if (game.GetAllIslands()[0].Owner != game.GetMyself())
                                Mover.MoveAircraft(pirate, game.GetAllIslands()[0], game);
                            else if (game.GetAllIslands()[2].Owner != game.GetMyself())
                                Mover.MoveAircraft(pirate, game.GetAllIslands()[2], game);
                            else 
                                Mover.MoveAircraft(pirate, game.GetMyCities()[0], game);
                        }
                    }
                }
                #endregion
                else // Tournament Bot
                {
                    if(!decoyed && game.GetMyself().TurnsToDecoyReload == 0)
                    {
                        game.Decoy(pirate);
                        decoyed = true;
                    }
                    else if(!Attacker.TryAttack(pirate, game))
                    {
                        int count = 0;
                        if (game.GetEnemyLivingPirates().OrderBy(p=>p.Distance(game.GetAllIslands()[3])).ToList()[0].InRange(pirate,5))
                        {
                            Mover.MoveAircraft(pirate, game.GetEnemyLivingPirates().OrderBy(p=>p.Distance(game.GetAllIslands()[3])).ToList()[0], game);
                        }
                        else
                        {
                            Mover.MoveAircraft(pirate, game.GetAllIslands()[3], game);
                        }
                    }
                }
            }
        }

        private void HandleDrones(PirateGame game)
        {
            // Go over all of my drones
            foreach (Drone drone in game.GetMyLivingDrones())
            {
                #region Week 1
                if(game.GetOpponentName() == "11999" || game.GetOpponentName() == "12000" || game.GetOpponentName() == "12001" || game.GetOpponentName() == "12002" || game.GetOpponentName() == "12003" || game.GetOpponentName() == "12004" || game.GetOpponentName() == "12005" || game.GetOpponentName() == "12006")
                {
                    if (Week1.GoAsUnit(game) || done || game.GetMyCities()[0].Location.Col != game.GetAllIslands()[0].Location.Col)
                    {
                        // Get my first city
                        Location destination = game.GetMyCities()[0].Location;
                        // Bee
                        if (game.GetMyCities()[0].Location.Col != game.GetAllIslands()[0].Location.Col && game.GetMyCities()[0].Location.Row == game.GetAllIslands()[0].Location.Row)
                        {
                            if (drone.Location.Row < game.GetRowCount() - 1 && drone.Location.Col == game.GetMyIslands()[0].Location.Col)
                                destination = new Location(game.GetRowCount() - 1, drone.Location.Col);
                            else if (drone.Location.Row != game.GetMyCities()[0].Location.Row)
                                destination = new Location(game.GetMyCities()[0].Location.Row, game.GetMyCities()[0].Location.Col - 4);
                        }
                        else
                            destination = game.GetMyCities()[0].Location;
                        // Move Drone
                        Mover.MoveAircraft(drone, destination, game);
                    }
                }
                #endregion
                
                else if (game.GetOpponentName() == "12217") // Week 3 Bot 1
                {
                    Location destination = new Location(0, 0);
                    if (drone.InitialLocation.Equals(game.GetAllIslands()[1].Location))
                        destination = new Location(8, 20);
                    else
                        destination = game.GetMyCities().OrderBy(c => c.Distance(drone)).ToList()[0].Location;
                    Mover.MoveAircraft(drone, destination, game);
                }
                else if (game.GetOpponentName() == "12218") // Week 3 Bot 2
                {
                    if (game.GetMyLivingDrones().Count <= 20)
                    {
                        Mover.MoveAircraft(drone, new Location(game.GetMyCities()[0].Location.Row - 4, game.GetMyCities()[0].Location.Col), game);
                    }
                    else
                        Mover.MoveAircraft(drone, game.GetMyCities()[0], game);
                }
                else if (game.GetOpponentName() == "12222")
                {
                    Mover.MoveAircraft(drone, game.GetMyCities()[0], game);
                }
                else if (game.GetOpponentName() == "12224")
                {
                    if (drone.InitialLocation.Equals(game.GetAllIslands()[1].Location))
                        Mover.MoveAircraft(drone, game.GetMyCities()[0], game);
                    else
                        Mover.MoveAircraft(drone, game.GetNeutralCities()[0], game);
                }
                else
                {
                    Location destination = new Location(0, 0);
                    if (game.GetNeutralCities().Count > 0)
                        destination = game.GetNeutralCities().OrderBy(c => c.Distance(drone)).ToList()[0].Location;
                    else
                        destination = game.GetMyCities().OrderBy(c => c.Distance(drone)).ToList()[0].Location;
                    // Move Drone
                    Mover.MoveAircraft(drone, destination, game);
                }
            }
        }
        
        private bool IsGroup(PirateGame game)
        {
            for(int i = 0; i < game.GetMyLivingPirates().Count - 1; i++)
            {
                if (i + 1 < game.GetMyLivingPirates().Count - 1 && !game.GetMyLivingPirates()[i].Location.Equals(game.GetMyLivingPirates()[i+1].Location))
                    return false;
            }
            return true;
        }

        private int PiratesWithBalls(PirateGame game)
        {
            int c = 0;
            foreach(Pirate p in game.GetMyLivingPirates())
            {
                if (p.HasPaintball)
                    c++;
            }
            return c;
        }
    }
}