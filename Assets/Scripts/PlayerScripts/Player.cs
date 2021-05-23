using System.Linq;
using DialogueSystem;
using Interfaces;
using PlayerScripts;
using UnityEngine;

public class Player : Singleton<Player>, IPlayer
{
    [SerializeField] private int health;
    [SerializeField] internal int maxHealth;
    [SerializeField] private int speed;
    [SerializeField] private float attackDuration;
    [SerializeField] private float comboAttackDuration;
    
    public Damage damage = new Damage(100, DamageType.Physic);
    public PlayerSave playerSave;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float circleRadius;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private SpriteRenderer sprite;
    
    private new Rigidbody2D rigidbody;
    private Animator animator;
    private InputMaster input;
    private Vector2 movement;
    private float lastTimeAttack;
    private int combo;

    private int level;
    private static readonly int StrongAttack = Animator.StringToHash("StrongAttack");
    private static readonly int Combo = Animator.StringToHash("Combo");
    private static readonly int AttackAnimation = Animator.StringToHash("Attack");
    private static readonly int Movement = Animator.StringToHash("Movement");
    private static readonly int DeadAnimation = Animator.StringToHash("Dead");
    private static readonly int RevivalAnimation = Animator.StringToHash("Revival");

    public int Level
    {
        get => level;
        set
        {
            level = value;
            UISystem.Instance.lvlInfo.text = $"{level} Lvl";
        }
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
    public int Experience { get; set; }

    private int NeedExperienceCurrent => 100 + 50 * Level;

    private void Start()
    {
        input = InputSystem.Instance.Input;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        input.Player.Move.performed += context => Move(context.ReadValue<Vector2>());
        input.Player.Move.canceled += context => Move(Vector3.zero);
        input.Player.Shot.performed += context => Attack();        
        input.Player.StrongAttack.performed += context => Attack(true);
        Health = maxHealth;
        Level = 1;
        playerSave.LoadData();
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = movement * speed;

        rotatePoint.rotation = Quaternion.Euler(0, 0, Physics.GetAngleToMouse(mainCamera, transform.position));
    }

    public void Attack(bool isStrongAttack = false)
    {
        if (DialogueManager.Instance.isTalk)
            return;
        if (isStrongAttack && Time.time - lastTimeAttack >= attackDuration)
        {
            animator.SetTrigger(StrongAttack);
        }
        else if (Time.time - lastTimeAttack >= attackDuration) 
        {
            combo = 1;
            animator.SetTrigger(AttackAnimation);
            animator.SetInteger(Combo, combo);
        }
        else if (Time.time - lastTimeAttack < comboAttackDuration)
        {
            switch (combo)
            {
                case 1:
                    combo = 2;
                    break;
                case 2:
                    combo = 3;
                    isStrongAttack = true;
                    break;
                case 3:
                    return;
            }
            animator.SetInteger(Combo, combo);
        }
        else
        {
            return;
        }
        
        lastTimeAttack = Time.time;
        var enemies = Physics.FindColliders(attackPoint.position, circleRadius, enemyLayers);
        var modifier = isStrongAttack ? 2 : 1;
        var angle = Physics.GetAngleToMouse(mainCamera, transform.position);
        sprite.flipX = angle > 0 || angle < -180;
        foreach (var enemy in enemies.Select(x => x.GetComponent<Enemy>()))
        {
            GameManager.Instance.ProceedDamage(this, enemy, damage * modifier);
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
        Health -= damageGet.Size;
    }

    public void Dead()
    {
        input.Player.Disable();
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger(DeadAnimation);
        UISystem.Instance.FadeIn(true);
    }

    public void Revival()
    {
        input.Player.Enable();
        GetComponent<Collider2D>().enabled = true;
        animator.SetTrigger(RevivalAnimation);
        UISystem.Instance.FadeOut();
    }

    public void Move(Vector2 inputMovement)
    {
        if (DialogueManager.Instance.isTalk)
            return;
        movement = inputMovement;
        animator.SetFloat(Movement, movement.magnitude);
        if (inputMovement.x != 0)
            sprite.flipX = inputMovement.x < 0;
    }
}