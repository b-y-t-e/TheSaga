using System;
using System.Globalization;

namespace TheSaga.ValueObjects
{
    public readonly struct ExecutionID : IEquatable<ExecutionID>
    {
        public Guid Value { get; }

        ////////////////////////////////
        public ExecutionID(Guid value)
        {
            Value = value;
        }

        ////////////////////////////////

        public static ExecutionID New()
        {
            return new ExecutionID(Guid.NewGuid());
        }

        public static ExecutionID Empty()
        {
            return new ExecutionID(Guid.Empty);
        }

        public static ExecutionID From(Guid id)
        {
            return new ExecutionID(id);
        }

        ////////////////////////////////

        public static implicit operator Guid(ExecutionID self)
        {
            return self.Value;
        }

        ////////////////////////////////

        public static bool operator ==(ExecutionID value1, ExecutionID value2)
        {
            return value1.Value.Equals(value2.Value);
        }

        public static bool operator !=(ExecutionID value1, ExecutionID value2)
        {
            return !value1.Value.Equals(value2.Value);
        }

        ////////////////////////////////
        public bool Equals(ExecutionID other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is ExecutionID other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Convert.ToString(Value, CultureInfo.InvariantCulture);
        }
    }
}