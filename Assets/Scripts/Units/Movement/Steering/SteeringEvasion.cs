namespace StateOfClone.Units
{
    public class SteeringEvasion : SteeringFlee, ISteeringBehavior
    {
        private readonly ISteeringPredictor _predictor;

        public SteeringEvasion(
            UnitData ud, Locomotion locomotion, ISteeringPredictor steeringPredictor
            ) : base(ud, locomotion)
        {
            _predictor = steeringPredictor;
            SteeringType = SteeringType.Evasion;
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
