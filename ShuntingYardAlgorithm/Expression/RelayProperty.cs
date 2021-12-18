using ShuntingYardAlgorithm.Exceptions;
using System;

namespace ShuntingYardAlgorithm.Expression
{
    public class RelayProperty : IProperty
    {
        private readonly Func<double>   _getter;
        private readonly Action<double> _setter;

        public bool CanRead { get; }

        /// <summary>
        /// If Write is called while this property is set to false, an exception will be thrown.
        /// </summary>
        public bool CanWrite { get; }

        /// <summary>
        /// name of the property
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a readonly property
        /// </summary>
        /// <param name="name">name of the property</param>
        /// <param name="getter">method to execute on-read</param>
        public RelayProperty(string name, Func<double> getter) : this(name, getter, null) { }
        
        /// <summary>
        /// Creates a read/write property.
        /// You can disable either read or write by setting the getter or setter to null. doing so will set CanRead or CanWrite to false.
        /// </summary>
        /// <param name="name">name of the property</param>
        /// <param name="getter">method to execute on-read</param>
        /// <param name="setter">method to execute on-write</param>
        public RelayProperty(string name, Func<double> getter, Action<double> setter) : this(name, getter != null, setter != null) 
        {
            _getter = getter;
            _setter = setter;
        }

        protected RelayProperty(string name, bool canRead, bool canWrite)
        {
            Name     = name;
            CanRead  = canRead;
            CanWrite = canWrite;
        }

        public double Read()
        {
            if (!CanRead) throw new PropertyException($"Trying to read from unreadable property '{Name}'");
            return _getter();
        }

        public void Write(double value)
        {
            if (!CanWrite) throw new PropertyException($"Trying to write to unwriteable property '{Name}'");
            _setter(value);
        }
    }
}
