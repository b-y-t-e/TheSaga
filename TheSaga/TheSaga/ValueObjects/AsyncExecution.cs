using System;

namespace TheSaga.ValueObjects
{
    public readonly struct AsyncExecution : IEquatable<AsyncExecution>
    {
        public Boolean Value { get; }

        ////////////////////////////////
        internal AsyncExecution(Boolean value)
            => Value = value;

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

        public static implicit operator Boolean(AsyncExecution self)
            => self.Value;

        ////////////////////////////////

        public static bool operator ==(AsyncExecution value1, AsyncExecution value2)
            => value1.Value.Equals(value2.Value);

        public static bool operator !=(AsyncExecution value1, AsyncExecution value2)
            => !value1.Value.Equals(value2.Value);

        ////////////////////////////////
        public bool Equals(AsyncExecution other) =>
            Value.Equals(other.Value);

        public override bool Equals(object obj) =>
            obj is AsyncExecution other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() =>
            Convert.ToString(Value, System.Globalization.CultureInfo.InvariantCulture);
    }
}
