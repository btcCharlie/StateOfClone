using UnityEngine;

namespace StateOfClone.Units
{
    public abstract class SteeringBehavior : ISteeringBehavior
    {
        public SteeringType SteeringType { get; protected set; }

        protected UnitData _ud;
        protected Locomotion _locomotion;

        public SteeringBehavior(UnitData ud, Locomotion locomotion)
        {
            _ud = ud;
            _locomotion = locomotion;
        }

        public abstract SteeringParams GetSteering(Vector3 position, Vector3 target);

        /// <summary>
        /// Calculate the angle difference between the current and 
        /// desired velocity along the world horizontal plane.
        /// </summary>
        /// <param name="desiredVelocity">Velocity towards the desired location</param>
        /// <returns></returns>
        protected abstract float CalculateYaw(Vector3 desiredVelocity);

        /// <summary>
        /// Calculate the angle difference between the current and 
        /// desired velocity along the world lateral plane.
        /// <br/>
        /// Relevant for airborne units.
        /// </summary>
        /// <param name="desiredVelocity">Velocity towards the desired location</param>
        /// <returns></returns>
        protected abstract float CalculatePitch(Vector3 desiredVelocity);

        /// <summary>
        /// Calculate the desired speed.
        /// </summary>
        /// <param name="desiredVelocity">Velocity towards the desired location</param>
        /// <param name="trueMaxSpeed"></param>
        /// <returns></returns>
        protected abstract float CalculateSpeed(Vector3 desiredVelocity, float trueMaxSpeed);
    }
}
