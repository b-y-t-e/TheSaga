using System;
using System.Globalization;

namespace TheSaga.ValueObjects
{
    public readonly struct AsyncExecution : IEquatable<AsyncExecution>
    {
        public bool Value { get; }

        ////////////////////////////////
        internal AsyncExecution(bool value)
        {
            Value = value;
        }

        ////////////////////////////////

        public static AsyncExecution False()
        {
            return new AsyncExecution(false);
        }

        public static AsyncExecution True()
        {
            return new AsyncExecution(true);
        }

        public static AsyncExecution From(bool val)
        {
            return new AsyncExecution(val);
        }

        ////////////////////////////////

        public static implicit operator bool(AsyncExecution self)
        {
            return self.Value;
        }

        ////////////////////////////////

        public static bool operator ==(AsyncExecution value1, AsyncExecution value2)
        {
            return value1.Value.Equals(value2.Value);
        }

        public static bool operator !=(AsyncExecution value1, AsyncExecution value2)
        {
            return !value1.Value.Equals(value2.Value);
        }

        ////////////////////////////////
        public bool Equals(AsyncExecution other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is AsyncExecution other && Equals(other);
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