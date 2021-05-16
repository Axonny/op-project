using UnityEngine;

namespace Interfaces
{
    internal interface IPlayer : IUnit, IMove
    {
        void Attack(bool isStrongAttack);
    }
}