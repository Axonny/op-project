using System;
using Interfaces;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy
{
    private int health;
    [SerializeField] private int maxHealth = 100;
    public float deadTime;

    private Animator animator;
    
    private static readonly int HitAnimation = Animator.StringToHash("Hit");

    private void Awake()
    {
        health = maxHealth;
        animator = gameObject.GetComponent<Animator>();
    }

    internal void GetDamage(int damage)
    {
        if (damage < 0)
            throw new ArgumentException();
        health -= damage;    
        animator.SetTrigger(HitAnimation);
        animator.SetInteger("Health", health);
        if (health <= 0)
        {
            Debug.Log("Enemy died");
            Destroy(gameObject, deadTime);
        }
    }

    public void Attack()
    {
        throw new NotImplementedException();
    }

    public void Follow(Vector2 playerPosition)
    {
        throw new NotImplementedException();
    }

    public void Dead()
    {
        throw new NotImplementedException();
    }
}
