namespace ShuntingYardAlgorithm.Expression
{
    public interface IProperty
    {
        bool CanRead { get; }
        bool CanWrite { get; }
        string Name { get; }
        double Read();
        void Write(double value);
    }
}
