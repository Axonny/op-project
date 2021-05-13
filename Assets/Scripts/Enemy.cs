using System;
using Interfaces;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy, IMove
{
    private int health;
    [SerializeField] private int maxHealth = 100;
    public float deadTime;

    public int level = 1;
    public int experience;
    [SerializeField] private Damage damage = new Damage(10, DamageType.Physic);
    [SerializeField] private float attackDuration;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float circleRadius;
    [SerializeField] private LayerMask playerLayer;

    private Animator animator;
    private float lastTimeAttack;

    private static readonly int HitAnimation = Animator.StringToHash("Hit");
    private static readonly int HealthProperty = Animator.StringToHash("Health");
    private static readonly int AttackAnimation = Animator.StringToHash("Attack");


    public int Health { 
        get => health; 
        set => health = value; 
    }

    public int Level
    {
        get => level;
        set => level = value;
    }
    public int Experience { get; set; }


    private void Awake()
    {
        health = maxHealth;
        animator = gameObject.GetComponent<Animator>();
    }

    public void Move(Vector2 movement)
    {
        throw new NotImplementedException();
    }

    public void AddExperience(int experience)
    {
    }

    public void GetDamage(Damage damageGet, IUnit player)
    {
        // if (damage < 0)
            // throw new ArgumentException();
        health -= damageGet.Size;
        animator.SetTrigger(HitAnimation);
        animator.SetInteger(HealthProperty, health);
    }

    public void Attack()
    {
        if (Time.time - lastTimeAttack < attackDuration)
            return;
        lastTimeAttack = Time.time;
        animator.SetTrigger(AttackAnimation);
        var player = Physics.FindCollider(attackPoint.position, circleRadius, playerLayer);
        if (player != null)
        {
            GameManager.Instance.ProceedDamage(this, player.GetComponent<Player>(), damage);
        }
    }

    public void Follow(Vector2 playerPosition)
    {
        throw new NotImplementedException();
    }

    public void Dead()
    {
        Debug.Log("Enemy died");
        GameManager.Instance.enemies.Remove(this);
        Destroy(gameObject, deadTime);
    }
}