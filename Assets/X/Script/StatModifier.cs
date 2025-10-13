
using System;

public enum StatModifierType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300
}

public readonly struct StatModifier : IEquatable<StatModifier>
{
    public readonly float Value;
    public readonly StatModifierType Type;
    public readonly int Order;
    public readonly object Source;

    private readonly int hashCode;

    public StatModifier(float value, StatModifierType type, int order, object source)
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
        hashCode = HashCode.Combine(Value, Type, Order, Source);
    }

    public StatModifier(float value, StatModifierType type) : this(value, type, (int)type, null) { }
    public StatModifier(float value, StatModifierType type, int order) : this (value, type, order, null) { }
    public StatModifier(float value, StatModifierType type, object source) : this(value, type, (int)type, source) { }

    public bool Equals(StatModifier other)
    {
        return Value == other.Value && Type == other.Type && Order == other.Order && Source == other.Source;
    }
    public override bool Equals(object obj)
    {
        return obj is StatModifier other && Equals(other);
    }

    public override int GetHashCode()
    {
        return hashCode;
    }
}
