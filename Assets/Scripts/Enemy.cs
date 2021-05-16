using System;
using Interfaces;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy, IMove
{
    private int health;
    [SerializeField] private int maxHealth = 100;
    public float deadTime;
    internal bool CanMove = true;

    public int level = 1;
    public int experience;
    [SerializeField] private Damage damage = new Damage(10, DamageType.Physic);
    [SerializeField] private float attackDuration;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float circleRadius;
    [SerializeField] private LayerMask playerLayer;

    private Animator animator;
    private float lastTimeAttack;
    private Player player;

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
        player = Player.Instance;
    }

    private void LateUpdate()
    {
        var vector = player.transform.position - transform.position;
        var angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg - 90f;
        rotatePoint.rotation = Quaternion.Euler(0, 0, angle);
        if (Vector3.Distance(player.transform.position, attackPoint.position) < circleRadius / 2)
        {
            CanMove = false;
            if (Time.time - lastTimeAttack >= attackDuration)
                Attack();
        }
        else
        {
            CanMove = true;
        }
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
        health -= damageGet.Size;
        animator.SetTrigger(HitAnimation);
        animator.SetInteger(HealthProperty, health);
    }

    public void Attack()
    {
        lastTimeAttack = Time.time;
        animator.SetTrigger(AttackAnimation);
        var playerCol = Physics.FindCollider(attackPoint.position, circleRadius, playerLayer);
        if (playerCol != null)
        {
            GameManager.Instance.ProceedDamage(this, player, damage);
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