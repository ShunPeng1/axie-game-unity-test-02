using System.Collections.Generic;

namespace Game
{
    public interface ISkillStrategy
    {
        EnemyState FindTarget(Dictionary<int, EnemyState> enemyStates, EnemyState lastTargetedEnemy, double currentBulletFlightTime);
        bool TryShootSkill(EnemyState target, out double currentBulletFlightTime);
    }
}