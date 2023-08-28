using System;

namespace StateOfClone.Units
{
    public enum SteeringType
    {
        Seek, Flee, Pursuit, Evasion, Arrival
    }

    public static class SteeringBehaviorFactory
    {
        public static ISteeringBehavior CreateBehavior(
            SteeringType steeringType, UnitData unitData, Locomotion locomotion
        )
        {
            return steeringType switch
            {
                SteeringType.Seek => CreateSteeringSeek(
                    unitData, locomotion
                ),
                SteeringType.Flee => CreateSteeringFlee(
                    unitData, locomotion
                ),
                _ => throw new NotImplementedException(steeringType.ToString())
            };
        }

        public static ISteeringBehavior CreateSteeringSeek(
            UnitData unitData, Locomotion locomotion
            )
        {
            return new SteeringSeek(unitData, locomotion);
        }

        public static ISteeringBehavior CreateSteeringFlee(
            UnitData unitData, Locomotion locomotion
            )
        {
            return new SteeringFlee(unitData, locomotion);
        }

        public static ISteeringBehavior CreateSteeringArrival(
            UnitData unitData, Locomotion locomotion
            )
        {
            return new SteeringArrival(unitData, locomotion);
        }
    }
}