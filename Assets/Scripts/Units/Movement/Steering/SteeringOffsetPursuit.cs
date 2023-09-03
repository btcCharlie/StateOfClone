using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringOffsetPursuit : SteeringSeek, ISteeringBehavior
    {
        private readonly ISteeringPredictor _predictor;

        public SteeringOffsetPursuit(
            UnitData ud, Locomotion locomotion, ISteeringPredictor steeringPredictor
            ) : base(ud, locomotion)
        {
            _predictor = steeringPredictor;
            SteeringType = SteeringType.OffsetPursuit;
        }

        public override SteeringParams GetSteering(SelectionInfo self, SelectionInfo target)
        {
            Debug.LogWarning("Offset Pursuit steering is WIP!");
            if (target.Type == SelectionType.Moveable)
            {
                SelectionInfo predictedTarget = _predictor.PredictPosition(self, target);

                Vector3 offsetTowardsSelf =
                    self.Moveable.transform.position - target.Moveable.transform.position;
                offsetTowardsSelf =
                    offsetTowardsSelf.normalized *
                    Mathf.Min(_ud.MainWeaponRange, offsetTowardsSelf.magnitude);

                float distance =
                    offsetTowardsSelf.magnitude > _ud.MainWeaponRange ?
                    _ud.MainWeaponRange : offsetTowardsSelf.magnitude;

                target = new(predictedTarget.Position + offsetTowardsSelf);
            }

            SteeringParams sp = base.GetSteering(self, target);
            sp.Target = target.Position;
            return sp;
        }
    }
}
