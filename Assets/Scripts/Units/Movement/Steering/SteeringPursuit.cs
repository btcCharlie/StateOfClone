using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringPursuit : SteeringSeek, ISteeringBehavior
    {
        private readonly ISteeringPredictor _predictor;

        public SteeringPursuit(
            UnitData ud, Locomotion locomotion, ISteeringPredictor steeringPredictor
            ) : base(ud, locomotion)
        {
            _predictor = steeringPredictor;
            SteeringType = SteeringType.Pursuit;
        }

        public override SteeringParams GetSteering(Vector3 position, TargetInfo target)
        {
            target = target.Type switch
            {
                TargetType.Selectable => _predictor.PredictPosition(position, target),
                _ => target
            };

            SteeringParams sp = base.GetSteering(position, target);
            sp.Target = target.Position;
            return sp;
        }
    }
}
