using StateOfClone.Core;

namespace StateOfClone.Units
{
    public static class SteeringBehaviorFactory
    {
        public static ISteeringBehavior CreateSteeringSeek(
            UnitData unitData, Locomotion locomotion
            )
        {
            return new SteeringSeek(unitData, locomotion);
        }

        public static ISteeringBehavior CreateSteeringArrival(
            UnitData unitData, Locomotion locomotion
            )
        {
            return new SteeringArrival(unitData, locomotion);
        }

        // Add more methods for other types of steering behaviors...
    }
}