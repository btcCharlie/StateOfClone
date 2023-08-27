namespace StateOfClone.Units
{
    public class TrackedSpeedCalculator : DefaultSpeedCalculator, ISpeedCalculator
    {
        public TrackedSpeedCalculator(UnitData ud) : base(ud)
        {
        }

        public new float GetMaxSpeedAtTurnRate(float turnRate)
        {
            if (turnRate >= TurnLimit)
            {
                return 0f;
            }

            return base.GetMaxSpeedAtTurnRate(turnRate);
        }
    }
}
