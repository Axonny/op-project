using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int health;
    [SerializeField] private int maxHealth = 100;
    
    private void Awake()
    {
        health = maxHealth;
    }

    internal void GetDamage(int damage)
    {
        if (damage < 0)
            throw new ArgumentException();
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Enemy died");
            Destroy(gameObject);
        }
    }
}
