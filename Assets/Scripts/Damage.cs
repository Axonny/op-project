using System;
using Interfaces;
using UnityEngine;

[Serializable]
public class Damage : IDamage
{
    [SerializeField] private int size;
    [SerializeField] private DamageType type;

    public int Size
    {
        get => size;
        set => size = value;
    }

    public DamageType Type
    {
        get => type;
        set => type = value;
    }

    public Damage(int size, DamageType type)
    {
        this.size = size;
        Type = type;
    }

    public static Damage operator *(Damage damage, float modifier)
    {
        if (Math.Abs(modifier - 1) < 10e-9)
            return damage;
        return new Damage((int)(damage.size * modifier), damage.type);
    }
}