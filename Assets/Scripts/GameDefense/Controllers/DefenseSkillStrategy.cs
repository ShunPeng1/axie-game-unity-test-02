using System.Collections.Generic;

namespace Game
{
    public abstract class DefenseSkillStrategy : ISkillStrategy
    {
        protected readonly DefenseState defenseState;
        
        protected int BULLET_ATK  => DefenseState.BULLET_ATK;
        protected float GRAVITY => DefenseState.GRAVITY;
        protected float FIXED_TIME_STEP => DefenseState.FIXED_TIME_STEP;

        public DefenseSkillStrategy(DefenseState defenseState)
        {
            this.defenseState = defenseState;
        }

        public abstract EnemyState FindTarget(Dictionary<int, EnemyState> enemyStates, EnemyState lastTargetedEnemy, double currentBulletFlightTime);

        public abstract bool TryShootSkill(EnemyState target, out double currentBulletFlightTime);
    }
}