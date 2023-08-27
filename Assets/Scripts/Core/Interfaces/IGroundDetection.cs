using UnityEngine;

namespace StateOfClone.Core
{
    public interface IGroundDetection
    {
        RaycastHit DetectGround(Vector3 position, LayerMask groundLayer);
    }
}
