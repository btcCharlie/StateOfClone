namespace StateOfClone.Units
{
    public interface ISpeedCalculator
    {
        float TurnLimit { get; set; }

        float CalculateSpeed(float speedDeviation, float maxSpeed, float minSpeed);

        float CalculateYawTurnRate(float yawDeviation);
        float CalculateYawTurnRate(float yawDeviation, float maxTurnRate, float minTurnRate);

        float GetMaxSpeedAtTurnRate(float turnRate);
    }
}
