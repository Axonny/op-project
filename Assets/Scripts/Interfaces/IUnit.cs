
namespace Interfaces
{
    public interface IUnit
    {
        int Health {get; set;}
        int Level {get; set;}
        int Experience {get; set;}
        bool IDead {get; set;}

        void AddExperience(int experience);
        void GetDamage(Damage damageGet, IUnit attacker);
        void Dead();
    }
}