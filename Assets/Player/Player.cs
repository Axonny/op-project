using System;
using DialogueSystem;
using Interfaces;
using UnityEngine;

public class Player :Singleton<Player>, IPlayer
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int speed;
    [SerializeField] private int damagePower;
    [SerializeField] private float attackDuration;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float circleRadius;
    [SerializeField] private LayerMask enemyLayers;

    private SpriteRenderer sprite;
    private new Rigidbody2D rigidbody;
    private Animator animator;
    private InputMaster input;
    private Vector2 movement;
    private float lastTimeAttack;
    
    private static readonly int AttackAnimation = Animator.StringToHash("Attack");


    public int Level { get; set; }

    public int Health
    {
        get => health;
        set
        {
            health = value;
            if (health > maxHealth)
                health = maxHealth;
            UISystem.Instance.healthBar.value = health;
        }
    }

    private void Start()
    {
        input = InputSystem.Instance.Input;
        sprite = gameObject.GetComponent<SpriteRenderer>();
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        input.Player.Move.performed += context => Move(context.ReadValue<Vector2>());
        input.Player.Move.canceled += context => movement = Vector3.zero;
        input.Player.Shot.performed += context => Attack();
        Health = maxHealth;
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = movement * speed;
        
        rotatePoint.rotation = Quaternion.Euler(0,0, Physics.GetAngleToMouse(mainCamera,transform.position));
    }

    public void Attack()
    {
        if(Time.time - lastTimeAttack < attackDuration || DialogueManager.Instance.isTalk)
            return;
        lastTimeAttack = Time.time;
        animator.SetTrigger(AttackAnimation);
        var enemies = Physics.FindColliders(attackPoint.position, circleRadius, enemyLayers);
        foreach (var enemy in enemies)
        {
            Debug.Log("hit");
            enemy.GetComponent<Enemy>().GetDamage(damagePower, this);
        }
    }

    public void GetDamage(int damage, IUnit enemy)
    {
        if (damage < 0)
            throw new ArgumentException();
        Health -= damage;
        if (health <= 0)
        {
            Dead(enemy);
        }
    }

    public void Dead(IUnit enemy)
    {
        Debug.Log("Player died");
        Destroy(gameObject);
    }

    public void Move(Vector2 inputMovement)
    {
        if (DialogueManager.Instance.isTalk)
            return;
        movement = inputMovement;
        if (inputMovement.x != 0)
            sprite.flipX = inputMovement.x < 0;
    }
}
