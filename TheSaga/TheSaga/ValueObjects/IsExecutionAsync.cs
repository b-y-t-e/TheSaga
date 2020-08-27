using System;

namespace TheSaga.Execution.Actions
{
    public readonly struct IsExecutionAsync : IEquatable<IsExecutionAsync>
    {
        public Boolean Value { get; }

        ////////////////////////////////
        internal IsExecutionAsync(Boolean value)
            => Value = value;

        ////////////////////////////////

        public static IsExecutionAsync False()
        {
            return new IsExecutionAsync(false);
        }

        public static IsExecutionAsync True()
        {
            return new IsExecutionAsync(true);
        }

        ////////////////////////////////

        public static implicit operator Boolean(IsExecutionAsync self)
            => self.Value;

        ////////////////////////////////

        public static bool operator ==(IsExecutionAsync value1, IsExecutionAsync value2)
            => value1.Value.Equals(value2.Value);

        public static bool operator !=(IsExecutionAsync value1, IsExecutionAsync value2)
            => !value1.Value.Equals(value2.Value);

        ////////////////////////////////
        public bool Equals(IsExecutionAsync other) =>
            Value.Equals(other.Value);

        public override bool Equals(object obj) =>
            obj is IsExecutionAsync other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() =>
            Convert.ToString(Value, System.Globalization.CultureInfo.InvariantCulture);
    }
}
