using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class AIController : MonoBehaviour
    {
        DefenseState _defenseState;

        [Header("Fixed Angle Implementation")]
        [SerializeField] private float _fixedAngle = 45;
        private double _bulletFlightTime = 0;
        private EnemyState _targetedEnemy;
        private float _accumulateTime;
        private List<DefenseSkillStrategy> _skillStrategies = new List<DefenseSkillStrategy>();
        

        
        [Header("Debug")]
        [SerializeField] private bool showDebug = false;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Vector2 _bulletOffset = new Vector3(0, 5, 0);
        private List<GameObject> _bullets = new List<GameObject>();
        
        private static float SHOOT_DELAY_TIME => DefenseState.SHOOT_DELAY_TIME;
        private static float FIXED_TIME_STEP => DefenseState.FIXED_TIME_STEP;
        
        
        private void Awake()
        {
            var manager = FindObjectOfType<GameDefenseManager>();
            if (manager == null)
            {
                this.enabled = false;
            }
            else
            {
                _defenseState = manager.GetState();

                // Add all the strategies with priority 
                _skillStrategies.Add(new SpecialShootSkillStrategy(_defenseState, _fixedAngle));
                _skillStrategies.Add(new NormalShootSkillStrategy(_defenseState, _fixedAngle)); 
            }

        }

        // Update is called once per frame
        void Update()
        {
            UpdateBulletFightTime();
            
            if (_defenseState == null || !_defenseState.isPlaying || _defenseState.playerState.shootDelay > 0) return;

            // Loop through all the strategies to find the target
            foreach (var strategy in _skillStrategies)
            {
                // Find the target
                var targetedEnemy = strategy.FindTarget(_defenseState.enemyStates, _targetedEnemy, _bulletFlightTime);
                double bulletFlightTime = 0;
                // Shoot the target
                if (strategy.TryShootSkill(targetedEnemy, out bulletFlightTime))
                {
                    _targetedEnemy = targetedEnemy;
                    _bulletFlightTime = bulletFlightTime;
                    
                    break;
                }
            }            
            
            
        }

        private void UpdateBulletFightTime()
        {
            
            _accumulateTime += Time.deltaTime;
            while (this._accumulateTime >= DefenseState.FIXED_TIME_STEP)
            {
                this._accumulateTime -= DefenseState.FIXED_TIME_STEP;
                
                if (_bulletFlightTime > 0)
                {
                    _bulletFlightTime -= FIXED_TIME_STEP;
                }
            }
        }


        
        #region DEBUG

        private void ShowPredictPath(float shootAngle, float power)
        {
            foreach (var bullet in _bullets)
            {
                Destroy(bullet);
            }

            _bullets.Clear();

            var predictBulletPaths = _defenseState.PredictBulletPath(shootAngle, power);

            foreach (var path in predictBulletPaths)
            {
                var bullet = Instantiate(_bulletPrefab, path + _bulletOffset, Quaternion.identity);
                _bullets.Add(bullet);
            }

            foreach (var p in _defenseState.enemyStates)
            {
                var enemy = p.Value;

                Vector2 futurePosition = new Vector2(enemy.pos.x - enemy.speed * SHOOT_DELAY_TIME, enemy.pos.y);

                var bullet = Instantiate(_bulletPrefab, _bulletOffset + futurePosition, Quaternion.identity);
                _bullets.Add(bullet);
            }
        }

        #endregion

        
    }
}
