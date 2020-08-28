using System;
using System.Globalization;

namespace TheSaga.ValueObjects
{
    public readonly struct SagaID : IEquatable<SagaID>
    {
        public Guid Value { get; }

        ////////////////////////////////
        internal SagaID(Guid value)
        {
            Value = value;
        }

        ////////////////////////////////

        public static SagaID New()
        {
            return new SagaID(Guid.NewGuid());
        }

        public static SagaID Empty()
        {
            return new SagaID(Guid.Empty);
        }

        public static SagaID From(Guid id)
        {
            return new SagaID(id);
        }

        /*public static SagaID True()
        {
            return new SagaID(true);
        }*/

        ////////////////////////////////

        public static implicit operator Guid(SagaID self)
        {
            return self.Value;
        }

        ////////////////////////////////

        public static bool operator ==(SagaID value1, SagaID value2)
        {
            return value1.Value.Equals(value2.Value);
        }

        public static bool operator !=(SagaID value1, SagaID value2)
        {
            return !value1.Value.Equals(value2.Value);
        }

        ////////////////////////////////
        public bool Equals(SagaID other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is SagaID other && Equals(other);
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