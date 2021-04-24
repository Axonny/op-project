namespace Interfaces
{
    public interface IUnit
    {
        int Health {get; set;}
        int Level {get; set;}
        //int Experience {get; set;}
        void GetDamage(int damage, IUnit unit);
        void Attack();
        void Dead(IUnit unit);
    }
}