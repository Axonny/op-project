using System;
using DialogueSystem;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class Player :Singleton<Player>, IPlayer
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int speed;
    
    [SerializeField] private Slider healthBar;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private int damage;
    [SerializeField] private float attackDuration;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float circleRadius;
    [SerializeField] private LayerMask enemyLayers;
    public event Action UseAction;
    
    private SpriteRenderer sprite;
    private new Rigidbody2D rigidbody;
    private Animator animator;
    
    private InputMaster input;
    private Vector2 movement;
    private float lastTimeAttack;
    
    private static readonly int AttackAnimation = Animator.StringToHash("Attack");

    private int Health
    {
        set
        {
            if (value < 0) 
                Debug.Log("death");
            
            health = value;
            if (health > maxHealth)
                health = maxHealth;
            healthBar.value = value;
        }
    }

    private void Awake()
    {
        input = new InputMaster();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        input.Player.Move.performed += context => Move(context.ReadValue<Vector2>());
        input.Player.Move.canceled += context => movement = Vector3.zero;
        input.Player.Shot.performed += context => Attack();
        input.Player.Action.performed += context => UseAction?.Invoke();
        Health = maxHealth;
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = movement * speed;

        var vector = mainCamera.ScreenToWorldPoint(input.Mouse.Move.ReadValue<Vector2>()) - transform.position;
        var angle = Mathf.Atan2(vector.y, vector.x);
        rotatePoint.rotation = Quaternion.Euler(0,0, angle * Mathf.Rad2Deg - 90f);
    }

    public void Attack()
    {
        if(Time.time - lastTimeAttack < attackDuration || DialogueManager.Instance.isTalk)
            return;
        lastTimeAttack = Time.time;
        animator.SetTrigger(AttackAnimation);
        var enemies = Physics2D.OverlapCircleAll(attackPoint.position, circleRadius, enemyLayers);
        foreach (var enemy in enemies)
        {
            Debug.Log("hit");
            enemy.GetComponent<Enemy>().GetDamage(damage);
        }
    }

    public void Dead()
    {
        throw new NotImplementedException();
    }

    public void Move(Vector2 inputMovement)
    {
        if (DialogueManager.Instance.isTalk)
            return;
        movement = inputMovement;
        if (inputMovement.x != 0)
            sprite.flipX = inputMovement.x < 0;
    }
    
    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
