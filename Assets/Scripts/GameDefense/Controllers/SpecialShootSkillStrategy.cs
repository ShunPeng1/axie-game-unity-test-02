using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class SpecialShootSkillStrategy : DefenseSkillStrategy
    {
        private readonly double _fixAngle;
        
        public SpecialShootSkillStrategy(DefenseState defenseState, double fixAngle) : base(defenseState)
        {
            _fixAngle = fixAngle;
        }
        
        public override EnemyState FindTarget(Dictionary<int, EnemyState> enemyStates, EnemyState lastTargetedEnemy, double currentBulletFlightTime)
        {
            Dictionary<EnemyState, int> enemyBulletToKill = new Dictionary<EnemyState, int>();
            int totalBulletToKill = 0;
            foreach (var p in defenseState.enemyStates)
            {
                var enemy = p.Value;
                
                // Skip if the enemy is the target and the bullet is still flying that is about to die soon
                if (enemy == lastTargetedEnemy && currentBulletFlightTime > 0 && enemy.hp <= BULLET_ATK)
                {
                    continue; 
                }
                
                int bulletToKill = (int)Mathf.Ceil(enemy.hp / BULLET_ATK);
                enemyBulletToKill.Add(enemy, bulletToKill);
                totalBulletToKill+= bulletToKill ;
            }
            
            // Skip if there are less than 2 enemies to shoot
            if (totalBulletToKill <= 2) return null; 

            EnemyState middleCusterEnemy = null; // There might be no enemy to shoot
            int totalLargestBulletToKill = 0;
            
            foreach (var (enemyState, shoot) in enemyBulletToKill)
            {
                int totalBulletToKillInCuster = shoot;
                
                // Count the total bullet to kill in the cluster
                foreach (var (enemyState2, shoot2) in enemyBulletToKill)
                {
                    if (enemyState == enemyState2) continue;
                    
                    // Check if the enemy is in range of the cluster
                    if (Mathf.Abs(enemyState.pos.x - enemyState2.pos.x) <= 0 && Mathf.Abs(enemyState.pos.y - enemyState2.pos.y) <= 1)
                    {
                        totalBulletToKillInCuster += shoot2;
                    }
                }
                
                // Skip if the total bullet to kill is less than 2, because it is not worth to shoot
                if (totalBulletToKillInCuster < 2) continue; 

                // Select the custer enemy that is nearest to the player and has the number of bullet to kill more than 3
                if ((middleCusterEnemy == null) ||enemyState.pos.x < middleCusterEnemy.pos.x && totalBulletToKillInCuster >= 3)
                {
                    middleCusterEnemy = enemyState;
                    totalLargestBulletToKill = totalBulletToKillInCuster;
                    
                    continue;
                }
                
                // Select the custer enemy that has the largest number of bullet to kill
                if (totalLargestBulletToKill < totalBulletToKillInCuster)
                {
                    middleCusterEnemy = enemyState;
                    totalLargestBulletToKill = totalBulletToKillInCuster;
                }
            }
            
            
            return middleCusterEnemy; // Might be null indicating no enemy worth to shoot

        }

        public override bool TryShootSkill(EnemyState target, out double currentBulletFlightTime)
        {
            if ( defenseState.energy >= DefenseState.ENERGY_SHOT_MAX_CHARGE && (target != null))
            {
                var result = ProjectileSolver.InverseFromStartFixAngle(defenseState.playerState.pos,target.pos, target.speed, _fixAngle);

                // The default power is when the target is too far away
                if (result.IsSuccess)
                {
                    defenseState.DoShootSpecial((float) _fixAngle, (float) result.Power);

                    currentBulletFlightTime = result.FlightTime;
                    return true;
                }
            }
            
            currentBulletFlightTime = 0;
            return false;
        }

    }
}