using System;
using Interfaces;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy, IMove
{
    private int health;
    [SerializeField] private int maxHealth = 100;
    public float deadTime;

    public bool isAlwaysAttack; // to delete
    
    [SerializeField] private int damagePower;
    [SerializeField] private float attackDuration;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float circleRadius;
    [SerializeField] private LayerMask playerLayer;

    private Animator animator;
    private float lastTimeAttack;
    
    private static readonly int HitAnimation = Animator.StringToHash("Hit");
    private static readonly int HealthProperty = Animator.StringToHash("Health");
    private static readonly int AttackAnimation = Animator.StringToHash("Attack");

    private void Awake()
    {
        health = maxHealth;
        animator = gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (isAlwaysAttack)
            Attack();
    }

    public void Move(Vector2 movement)
    {
        throw new NotImplementedException();
    }

    public void GetDamage(int damage)
    {
        if (damage < 0)
            throw new ArgumentException();
        health -= damage;    
        animator.SetTrigger(HitAnimation);
        animator.SetInteger(HealthProperty, health);
        if (health <= 0)
        {
            Dead();
        }
    }

    public void Attack()
    {
        if(Time.time - lastTimeAttack < attackDuration)
            return;
        lastTimeAttack = Time.time;
        animator.SetTrigger(AttackAnimation);
        var player = Physics.FindCollider(attackPoint.position, circleRadius, playerLayer);
        if (player != null)
        {
            player.GetComponent<Player>().GetDamage(damagePower);
        }
    }    

    public void Follow(Vector2 playerPosition)
    {
        throw new NotImplementedException();
    }

    public void Dead()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject, deadTime);
    }
}
