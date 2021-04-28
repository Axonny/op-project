using Interfaces;

public class Damage : IDamage
{
    public int size { get; set; }
    public DamageType Type { get; set; }

    public Damage(int size, DamageType type)
    {
        this.size = size;
        Type = type;
    }
}