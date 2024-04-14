using UnityEngine;

namespace Game
{
    public class ProjectileSolver
    {
        private static float POWER_MIN => DefenseState.POWER_MIN;
        private static float POWER_BOOST_MAX => DefenseState.POWER_BOOST_MAX;
        private static float GRAVITY => DefenseState.GRAVITY;
        private static float FIXED_TIME_STEP => DefenseState.FIXED_TIME_STEP;

        public class Result
        {
            public bool IsSuccess;
            public double Angle;
            public double Power;
            public double FlightTime;
            
            public Result(bool isSuccess,double angle, double power, double flightTime)
            {
                IsSuccess = isSuccess;
                Angle = angle;
                Power = power;
                FlightTime = flightTime;
            }
        }
        
        public static double VelocityToPower(double velocity)
        {
            return (velocity - POWER_MIN) * 100 / POWER_BOOST_MAX;
        }
        
        public static bool IsConvertableToPower(double velocity)
        {
            return (velocity - POWER_MIN)/ POWER_BOOST_MAX >= 0 && (velocity - POWER_MIN)/ POWER_BOOST_MAX <= 1;
        }
        
        public static Result InverseFromStartFixAngle(Vector2 from, Vector2 to, double enemySpeed, double fixedAngle)
        {
            float diffHeight = -(from.y - to.y);
            float diffDistance = (to.x - from.x);
            float g = GRAVITY;

            
            var sinAngle = Mathf.Sin((float)(fixedAngle * Mathf.Deg2Rad));
            var sin2Angle = Mathf.Sin((float)(2 * fixedAngle * Mathf.Deg2Rad));
            var cosAngle = Mathf.Cos( (float)(fixedAngle * Mathf.Deg2Rad));
            
            var a = sinAngle * cosAngle * diffDistance - diffHeight * cosAngle * cosAngle - 0.5 * g * diffDistance * FIXED_TIME_STEP * cosAngle;
            var b = diffDistance * enemySpeed * sinAngle - 2 * diffHeight * enemySpeed * cosAngle  - 0.5 * g * diffDistance* diffDistance - 0.5 * g * diffDistance * FIXED_TIME_STEP * enemySpeed;
            var c = - diffHeight * enemySpeed * enemySpeed ;

            var velocity = PolynomialSolver.SingleQuadratic(a,b,c);
            
            //Debug.Log("Result: " + velocity + " from a: " + a + " b: " + b + " c: " + c+ " with angle: " + fixedAngle + " and enemy speed: " + enemySpeed + " and diffDistance: " + diffDistance + " and diffHeight: " + diffHeight + " and g: " + g + " and sinAngle: " + sinAngle + " and cosAngle: " + cosAngle);
            
            
            if (velocity.Item2.Imaginary == 0 && IsConvertableToPower(velocity.Item2.Real))
            {
                var bulletVelocity = velocity.Item2.Real;
                var power = VelocityToPower(bulletVelocity);
                var bulletFlightTime = diffDistance / (bulletVelocity * cosAngle + enemySpeed);

                var result = new Result(true, fixedAngle, power, bulletFlightTime);
                
                //Debug.Log("Velocity: " + bulletVelocity + " for power: " + power + " with flight time: " + _bulletFlightTime);
                return result;
                
            }

            if (velocity.Item1.Imaginary == 0 && IsConvertableToPower(velocity.Item1.Real))
            {
                var bulletVelocity = velocity.Item1.Real;
                var power = VelocityToPower(bulletVelocity);
                
                var bulletFlightTime = diffDistance / (bulletVelocity * cosAngle + enemySpeed);

                var result = new Result(true, fixedAngle, power, bulletFlightTime);

                
                // Debug.Log("Velocity: " + bulletVelocity + " for power: " + power + " with flight time: " + _bulletFlightTime);
                return result;
                
            }

            // Default power
            
            return new Result(false, fixedAngle, 100, 0);
            
        }
        
    }
}