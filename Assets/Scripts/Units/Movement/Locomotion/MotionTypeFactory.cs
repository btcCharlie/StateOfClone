using System;

namespace StateOfClone.Units
{
    public static class MotionTypeFactory
    {
        public static IMotionType CreateMotion(MotionType motionType, UnitData unitData)
        {
            return motionType switch
            {
                MotionType.Wheeled => CreateWheeledMotion(unitData),
                MotionType.Tracked => CreateTrackedMotion(unitData),
                _ => throw new NotImplementedException(),
            };
        }

        private static IMotionType CreateWheeledMotion(UnitData unitData)
        {
            return new MotionWheeled(unitData);
        }

        public static IMotionType CreateTrackedMotion(UnitData unitData)
        {
            return new MotionTracked(unitData);
        }
    }
}
