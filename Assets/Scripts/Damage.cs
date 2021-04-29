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
}