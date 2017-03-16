using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class Attacker
    {
        public static bool TryAttack(Pirate pirate, PirateGame game)
        {
            if(game.GetOpponentName() == "12220" || game.GetOpponentName() == "12224" || game.GetOpponentName() == "12109" || game.GetOpponentName() == "12111")
            {
                            // Go over all enemies
                foreach (Aircraft enemy in game.GetEnemyLivingAircrafts())
                {
                    // Check if the enemy is in attack range and he's not our decoy
                    if (enemy != game.GetMyDecoy() && pirate.InAttackRange(enemy))
                    {
                        // Fire!
                        game.Attack(pirate, enemy);
                        // Print a message
                        game.Debug("pirate " + pirate + " attacks " + enemy);
                        // Did attack
                        return true;
                    }
                }
            }
            else
            {
                // Go over all enemy drones
                foreach (Drone enemy in game.GetEnemyLivingDrones())
                {
                    // Check if the enemy is in attack range and he's not our decoy
                    if (pirate.InAttackRange(enemy))
                    {
                        // Fire!
                        game.Attack(pirate, enemy);
                        // Print a message
                        game.Debug("pirate " + pirate + " attacks " + enemy);
                        // Did attack
                        return true;
                    }
                }
                //Go over all enemy pirates
                foreach (Pirate enemy in game.GetEnemyLivingPirates())
                {
                    // Check if the enemy is in attack range and he's not our decoy
                    if (enemy != game.GetMyDecoy() && pirate.InAttackRange(enemy))
                    {
                        // Fire!
                        game.Attack(pirate, enemy);
                        // Print a message
                        game.Debug("pirate " + pirate + " attacks " + enemy);
                        // Did attack
                        return true;
                    }
                }
            }

            // Didn't attack
            return false;
        }
        
        public static bool TryAttack(Pirate pirate, PirateGame game, Pirate enemy)
        {
            // Check if the enemy is in attack range and he's not our decoy
            if (pirate.InAttackRange(enemy))
            {
                 // Fire!
                 game.Attack(pirate, enemy);
                 // Print a message
                 game.Debug("pirate " + pirate + " attacks " + enemy);
                 // Did attack
                 return true;
            }
            // Didn't attack
            return false;
        }
    }
}
