using UnityEngine;
using System.Collections.Generic;

namespace StateOfClone.Units
{
    public struct SteeringParams
    {
        public Vector3 Steering { get; set; }
        public Vector3 DirectionToTarget { get; set; }
        public float Thrust { get; set; }

        public SteeringParams(Vector3 steering, Vector3 directionToTarget, float thrust)
        {
            Steering = steering;
            DirectionToTarget = directionToTarget;
            Thrust = thrust;
        }

        public static SteeringParams Zero => new(Vector3.zero, Vector3.zero, 0f);

        /// <summary>
        /// Adds two SteeringParams together by averaging the Steering and Thrust values.
        /// DirectionToTarget must be the same for both SteeringParams, else an 
        /// argument exception is thrown.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Throws an ArgumentException if the structs don't have same directionToTarget</exception>
        public static SteeringParams operator +(SteeringParams a, SteeringParams b)
        {
            if (a.DirectionToTarget != b.DirectionToTarget)
            {
                // throw an exception here
                throw new System.ArgumentException("DirectionToTarget must be the same for both SteeringParams!");
            }

            return new SteeringParams(
                (a.Steering + b.Steering) / 2f,
                a.DirectionToTarget,
                (a.Thrust + b.Thrust) / 2f
            );
        }

        /// <summary>
        /// Averages the Steering and Thrust values of the given SteeringParams array.
        /// </summary>
        /// <param name="steeringParams">Array of SteeringParam structs.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Throws an ArgumentException if the structs don't have same directionToTarget</exception>
        public static SteeringParams Average(SteeringParams[] steeringParams)
        {
            Vector3 steering = steeringParams[0].Steering;
            Vector3 directionToTarget = steeringParams[0].DirectionToTarget;
            float thrust = steeringParams[0].Thrust;

            for (int i = 1; i < steeringParams.Length; i++)
            {
                steering += steeringParams[i].Steering;
                thrust += steeringParams[i].Thrust;
                if (directionToTarget != steeringParams[i].DirectionToTarget)
                    throw new System.ArgumentException(
                        "DirectionToTarget must be the same for all SteeringParams!"
                    );
            }

            return new SteeringParams(
                steering / steeringParams.Length,
                directionToTarget,
                thrust / steeringParams.Length
            );
        }

        /// <summary>
        /// Averages the Steering and Thrust values of the given SteeringParams array.
        /// </summary>
        /// <param name="steeringParams">List of SteeringParam structs.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Throws an ArgumentException if the structs don't have same directionToTarget</exception>
        public static SteeringParams Average(List<SteeringParams> steeringParams)
        {
            Vector3 steering = steeringParams[0].Steering;
            Vector3 directionToTarget = steeringParams[0].DirectionToTarget;
            float thrust = steeringParams[0].Thrust;

            for (int i = 1; i < steeringParams.Count; i++)
            {
                steering += steeringParams[i].Steering;
                thrust += steeringParams[i].Thrust;
                if (directionToTarget != steeringParams[i].DirectionToTarget)
                    throw new System.ArgumentException(
                        "DirectionToTarget must be the same for all SteeringParams!"
                    );
            }

            return new SteeringParams(
                steering / steeringParams.Count,
                directionToTarget,
                thrust / steeringParams.Count
            );
        }
    }
}
