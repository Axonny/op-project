using UnityEngine;

namespace Interfaces
{
    public interface IEnemy : IUnit
    {
        void Follow(Vector2 playerPosition);
    }
}