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

        public override SteeringParams GetSteering(SelectionInfo self, SelectionInfo target)
        {
            target = target.Type switch
            {
                SelectionType.Moveable => _predictor.PredictPosition(self, target),
                _ => target
            };

            SteeringParams sp = base.GetSteering(self, target);
            sp.Target = target.Position;
            return sp;
        }
    }
}
