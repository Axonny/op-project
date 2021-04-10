using UnityEngine;

namespace Interfaces
{
    public interface IEnemy
    {
        void Attack();
        void Follow(Vector2 playerPosition);
        void Dead();
    }
}