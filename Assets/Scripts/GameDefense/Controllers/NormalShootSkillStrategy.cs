using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class NormalShootSkillStrategy : DefenseSkillStrategy
    {
        private readonly double _fixAngle;
        
        public NormalShootSkillStrategy(DefenseState defenseState, double fixAngle) : base(defenseState)
        {
            _fixAngle = fixAngle;
        }   
        
        public override EnemyState FindTarget(Dictionary<int, EnemyState> enemyStates, EnemyState lastTargetedEnemy, double currentBulletFlightTime)
        {
            EnemyState nearestEnemy = null;
            float nearestDist = 999;
            
            
            foreach (var p in enemyStates)
            {
                var enemy = p.Value;
                
                // Debug.Log("Enemy: " + enemy.pos.x + " " + enemy.pos.y + " currentBulletFlightTime: " + _bulletFlightTime);
                // Skip if the enemy is the target and the bullet is still flying that is about to die soon
                if (enemy == lastTargetedEnemy && currentBulletFlightTime > 0 && enemy.hp <= BULLET_ATK)
                {
                    continue; 
                }
                
                // Select the nearest enemy
                if (enemy != null && enemy.pos.x < nearestDist && enemy.pos.x > 0)
                {
                    nearestDist = enemy.pos.x;
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }

        public override bool TryShootSkill(EnemyState target, out double currentBulletFlightTime)
        {
            if (target == null)
            {
                currentBulletFlightTime = 0;
                return false;
            }

            var result = ProjectileSolver.InverseFromStartFixAngle(defenseState.playerState.pos,target.pos, target.speed, _fixAngle);
            
            defenseState.DoShoot((float)_fixAngle, (float)  result.Power);

            // We can always preemptively shoot the bullet to the furthest position 
            currentBulletFlightTime = result.FlightTime;
            return true; 
        }
        
    }
}