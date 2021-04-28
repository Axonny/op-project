﻿
namespace Interfaces
{
    public interface IUnit
    {
        int Health {get; set;}
        int Level {get; set;}
        int Experience {get; set;}

        void AddExperience(int experience);
        void GetDamage(Damage damage, IUnit unit);
        void Attack();
        void Dead();
    }
}