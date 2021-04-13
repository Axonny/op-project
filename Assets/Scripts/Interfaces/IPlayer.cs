﻿using UnityEngine;

namespace Interfaces
{
    internal interface IPlayer
    {
        void Move(Vector2 inputMovement);
        void Attack();
        void GetDamage(int damage);
        void Dead();
    }
}