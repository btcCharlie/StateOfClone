using UnityEngine;

namespace StateOfClone.Units
{
    public class SpeedManager
    {
        private readonly float YAWCURVESCALE = 3f;
        private readonly float SPEEDCURVESLANT = 0.1f;

        private float actualMaxSpeed;

        private UnitData _ud;

        public SpeedManager(UnitData unitData)
        {
            _ud = unitData;
            YAWCURVESCALE = unitData.YawCurveScale;
            SPEEDCURVESLANT = unitData.SpeedCurveSlant;
        }

        public float CalculateSpeed(float deviation, float maxSpeed)
        {
            // Implement speed calculation logic here
            return 0;
        }

        public float CalculateAngularSpeed(float deviation, float maxAngularSpeed)
        {
            return 0f;
        }

        private float GetSpeed(float speedDeviation)
        {
            return Mathf.Clamp(
                speedDeviation,
                -actualMaxSpeed,
                actualMaxSpeed
                );
        }

        /// <summary>
        /// Converts the provided angle deviation in degrees to a turn rate in
        /// degrees per second. Uses a polynomial expression: 
        /// (minTurn - maxTurn) * (1 - yawDeviation / maxTurn) ^ yawCurveScale + maxTurn
        /// Prevents slow turning speeds at low deviations.
        /// </summary>
        /// <param name="yawDeviation">The steering signal for horizontal turning</param>
        /// <returns>The actual turn rate of the vehicle bound by its limits</returns>
        public float GetTurnRateFromDeviation(float yawDeviation)
        {
            float unboundTurnRate = yawDeviation switch
            {
                float when yawDeviation > 0 =>
                    ((_ud.MinTurnRate - _ud.MaxTurnRate) *
                    Mathf.Pow(
                        1 - yawDeviation / _ud.MaxTurnRate,
                        YAWCURVESCALE
                        )) +
                    _ud.MaxTurnRate,
                float when yawDeviation < 0 =>
                    (_ud.MaxTurnRate - _ud.MinTurnRate) *
                    Mathf.Pow(
                        1 + yawDeviation / _ud.MaxTurnRate,
                        YAWCURVESCALE
                        ) -
                    _ud.MaxTurnRate,
                _ => 0f,
            };

            return Mathf.Clamp(
                unboundTurnRate,
                 -_ud.MaxTurnRate,
                 _ud.MaxTurnRate
                );
        }
    }
}
