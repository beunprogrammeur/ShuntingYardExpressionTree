using ShuntingYardAlgorithm.Exceptions;

namespace ShuntingYardAlgorithm.Expression
{
    public class ValueProperty : IProperty
    {
        private double _value;

        public bool CanRead { get; }

        public bool CanWrite { get; }

        public string Name { get; }

        public ValueProperty(string name, double initialValue = 0, bool canRead = true, bool canWrite = true)
        {
            Name = name;
            _value = initialValue;
            CanRead = canRead;
            CanWrite = canWrite;
        }

        public double Read()
        {
            if (!CanRead) throw new PropertyException($"Trying to read from unreadable property '{Name}'");
            return _value;
        }

        public void Write(double value)
        {
            if (!CanWrite) throw new PropertyException($"Trying to write to unwriteable property '{Name}'");
            _value = value;
        }
    }
}
