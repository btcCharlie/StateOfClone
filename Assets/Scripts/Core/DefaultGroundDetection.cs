using UnityEngine;

namespace StateOfClone.Core
{
    public class DefaultGroundDetection : IGroundDetection
    {
        public RaycastHit DetectGround(Vector3 position, LayerMask groundLayer)
        {
            // Implement your ground detection logic
            return new RaycastHit();
        }
    }
}
