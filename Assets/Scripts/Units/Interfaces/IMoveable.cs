namespace StateOfClone.Units
{
    public interface IMoveable
    {
        float CurrentSpeed { get; }
        UnitMove UnitMove { get; }
    }
}
