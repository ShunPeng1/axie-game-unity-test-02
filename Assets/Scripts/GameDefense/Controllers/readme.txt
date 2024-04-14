The implementation in the AIController class is as follows:
1. It use fixed angle for shooting the bullet, which is a serialized field in the inspector.
2. Two SkillStrategy class is created and sorted by the priority of the skill (SpecialShootSkillStrategy then NormalShootSkillStrategy). Choosing the first valid skill to use based on the check.
3. The debug was used to check on the projectile angle and the direction of the bullet. But it was reorganized for not using them because I only send the zip file and not the unity scene, you can implement it yourself

The SpecialShootSkillStrategy class choose the enemies that can take the most damage from the bullet and shoot the bullet to the enemy that can take the most damage in a cluster of enemy. It also ignore if there is no enemy or not worth enough cluster. 
The NormalShootSkillStrategy class choose the enemies that is the nearest to the player and shoot the bullet to the nearest enemy. It also preemptively shoot the bullet to the furthest right if there is no enemy.

Both of the SkillStrategy class use the ProjectileSolver class to calculate the power of the bullet based on the target position and speed. They also ignore the enemy that is going to dead in the last shot

The ProjectileSolver class is used to calculate the power of the bullet based on the target position and speed.

The PolynomialSolver class is used to solve the polynomial equation.
