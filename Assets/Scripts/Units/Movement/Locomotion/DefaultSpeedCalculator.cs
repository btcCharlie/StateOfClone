using UnityEngine;

namespace StateOfClone.Units
{
    public class DefaultSpeedCalculator : ISpeedCalculator
    {
        protected UnitData _ud;
        protected float _midTurnRate;
        protected float _speedStretch;

        public float TurnLimit { get; set; }

        public DefaultSpeedCalculator(UnitData ud)
        {
            _ud = ud;
            _midTurnRate = (_ud.MaxTurnRate - _ud.MinTurnRate) / 2f + _ud.MinTurnRate;
            float expKMinMid = Mathf.Exp(
                _ud.SpeedCurveSlant * (_ud.MinTurnRate - _midTurnRate)
                );
            _speedStretch =
                2f * (_ud.MinSpeed - _ud.MaxSpeed) * expKMinMid / (expKMinMid - 1f);
        }

        public float CalculateSpeed(float speedDeviation, float maxSpeed, float minSpeed)
        {
            return Mathf.Clamp(speedDeviation, -maxSpeed, maxSpeed);
        }

        /// <summary>
        /// Converts the provided angle deviation in degrees to a turn rate in
        /// degrees per second. Uses a polynomial expression: 
        /// (minTurn - maxTurn) * (1 - yawDeviation / maxTurn) ^ yawCurveScale + maxTurn
        /// Prevents slow turning speeds at low deviations.
        /// </summary>
        /// <param name="yawDeviation">The steering signal for horizontal turning</param>
        /// <returns>The actual turn rate of the vehicle bound by its limits</returns>
        public float CalculateYawTurnRate(float yawDeviation, float maxTurnRate, float minTurnRate)
        {
            float unboundTurnRate = yawDeviation switch
            {
                float when yawDeviation > 0 =>
                    ((_ud.MinTurnRate - _ud.MaxTurnRate) *
                    Mathf.Pow(
                        1 - yawDeviation / _ud.MaxTurnRate,
                        _ud.YawCurveScale
                        )) +
                    _ud.MaxTurnRate,
                float when yawDeviation < 0 =>
                    (_ud.MaxTurnRate - _ud.MinTurnRate) *
                    Mathf.Pow(
                        1 + yawDeviation / _ud.MaxTurnRate,
                        _ud.YawCurveScale
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

        public float CalculateYawTurnRate(float yawDeviation)
        {
            return CalculateYawTurnRate(
                yawDeviation, _ud.MaxTurnRate, -_ud.MaxTurnRate
                );
        }

        public float GetMaxSpeedAtTurnRate(float turnRate)
        {
            turnRate = Mathf.Clamp(
                Mathf.Abs(turnRate), _ud.MinTurnRate, _ud.MaxTurnRate
                );

            return
                (_ud.MaxSpeed - _ud.MinSpeed + _speedStretch) /
                (1f + Mathf.Exp(-_ud.SpeedCurveSlant * (-turnRate + _midTurnRate))) +
                _ud.MinSpeed - _speedStretch / 2f;
        }

        public float GetMaxSpeedAtTurnRate(float turnRate, float limit)
        {
            if (turnRate >= limit)
            {
                return 0f;
            }

            return GetMaxSpeedAtTurnRate(turnRate);
        }
    }

}
