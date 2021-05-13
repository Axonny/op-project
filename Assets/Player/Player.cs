using System;
using System.Linq;
using DialogueSystem;
using Interfaces;
using UnityEngine;

public class Player : Singleton<Player>, IPlayer
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int speed;
    [SerializeField] private float attackDuration;
    
    public Damage damage = new Damage(100, DamageType.Physic);

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

    private int NeedExperienceCurrent => 100 + 50 * Level;

    private static readonly int AttackAnimation = Animator.StringToHash("Attack");

    private int experience;
    private int level;
    public int Level
    {
        get => level;
        set
        {
            level = value;
            UISystem.Instance.lvlInfo.text = $"{level} Lvl";
        }
    }

    public int Experience
    {
        get => experience;
        set => experience = value;
    }

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
        sprite = gameObject.GetComponentInChildren<SpriteRenderer>();
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        input.Player.Move.performed += context => Move(context.ReadValue<Vector2>());
        input.Player.Move.canceled += context => movement = Vector3.zero;
        input.Player.Shot.performed += context => Attack();
        Health = maxHealth;
        Level = 1;
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = movement * speed;

        rotatePoint.rotation = Quaternion.Euler(0, 0, Physics.GetAngleToMouse(mainCamera, transform.position));
    }

    public void Attack()
    {
        if (Time.time - lastTimeAttack < attackDuration || DialogueManager.Instance.isTalk)
            return;
        lastTimeAttack = Time.time;
        animator.SetTrigger(AttackAnimation);
        var enemies = Physics.FindColliders(attackPoint.position, circleRadius, enemyLayers);
        foreach (var enemy in enemies.Select(x => x.GetComponent<Enemy>()))
        {
            Debug.Log("hit");
            GameManager.Instance.ProceedDamage(this, enemy, damage);
        }
    }

    public void AddExperience(int additionalExperience)
    {
        var newExperience = Experience + additionalExperience;
        while (newExperience >= NeedExperienceCurrent)
        {
            newExperience -= NeedExperienceCurrent;
            Level++;
        }

        Experience = newExperience;
        Debug.Log("Exp: " + Experience + " \\ " + NeedExperienceCurrent);
        Debug.Log("Level: " + Level);
    }

    public void GetDamage(Damage damageGet, IUnit enemy)
    {
        // if (damage < 0)
        // throw new ArgumentException();
        Health -= damageGet.Size;
    }

    public void Dead()
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