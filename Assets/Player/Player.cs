using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using Interfaces;
using UnityEditor;
using UnityEngine;

public class Player : Singleton<Player>, IPlayer
{
    internal Characteristic[] _characteristics =
    {
        new Characteristic("strength", 10),
        new Characteristic("vitality", 10),
        new Characteristic("agility", 10),
        new Characteristic("intelligence", 10),
        new Characteristic("wisdom", 10),
    };

    internal int freeSkillPoints = 0;
    internal int skillPointsPerLevel = 5;


    internal int experience;
    internal int level;

    internal int NeedExperienceCurrent => 100 + 50 * Level;

    internal int strongAttackModifier = 2;
    internal int strengthToDamageModifier = 3;
    internal int intelligenceToDamageModifier = 3;
    internal int vitalityToHealthModifier = 3;
    internal int wisdomToManaModifier = 3;
    internal int wisdomToManaRestoreModifier = 3;

    internal int MaxHealth => vitalityToHealthModifier * _characteristics[1].value;

    [SerializeField] private int health;
    [SerializeField] private int speed;
    [SerializeField] private float attackDuration;
    [SerializeField] private float comboAttackDuration;

    public Damage damage;

    public Damage SimpleDamage => new Damage(_characteristics[0].value * strengthToDamageModifier, DamageType.Physic);

    public Damage StrongDamage => new Damage(SimpleDamage.Size * strongAttackModifier, DamageType.Physic);

    public Damage MagickDamage =>
        new Damage(_characteristics[3].value * intelligenceToDamageModifier, DamageType.Physic);

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
    public int combo = 0;


    private static readonly int StrongAttack = Animator.StringToHash("StrongAttack");
    private static readonly int Combo = Animator.StringToHash("Combo");
    private static readonly int AttackAnimation = Animator.StringToHash("Attack");
    private static readonly int Movement = Animator.StringToHash("Movement");

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
            if (health > MaxHealth)
                health = MaxHealth;
            UISystem.Instance.healthBar.value = health * 1.0f / MaxHealth * 100;
        }
    }

    private void Start()
    {
        input = InputSystem.Instance.Input;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        input.Player.Move.performed += context => Move(context.ReadValue<Vector2>());
        input.Player.Move.canceled += context => Move(Vector3.zero);
        input.Player.Shot.performed += context => Attack();
        input.Player.StrongAttack.performed += context => Attack(true);
        Health = MaxHealth;
        Level = 1;
        for (int i = 0; i < _characteristics.Length; i++)
        {
            UISystem.Instance.PanelUIContainer.characteristic[i].text = _characteristics[i].value.ToString();
        }

        UISystem.Instance.PanelUIContainer.lvlInfo.text = level.ToString();
        UISystem.Instance.PanelUIContainer.currentExperience.text = experience.ToString();
        UISystem.Instance.PanelUIContainer.needExperience.text = NeedExperienceCurrent.ToString();
        UISystem.Instance.PanelUIContainer.freeSkillPoints.text = freeSkillPoints.ToString();
        UISystem.Instance.PanelUIContainer.simpleAttackDamage.text = SimpleDamage.ToString();
        UISystem.Instance.PanelUIContainer.strongAttackDamage.text = StrongAttack.ToString();
        UISystem.Instance.PanelUIContainer.magickAttackDamage.text = MagickDamage.ToString();
        UISystem.Instance.PanelUIContainer.maxHealth.text = MaxHealth.ToString();
        UISystem.Instance.PanelUIContainer.maxMana.text = freeSkillPoints.ToString();
        UISystem.Instance.PanelUIContainer.manaRestore.text = freeSkillPoints.ToString();
        UISystem.Instance.PanelUIContainer.movementSpeed.text = freeSkillPoints.ToString();
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
        var angle = Physics.GetAngleToMouse(mainCamera, transform.position);
        sprite.flipX = angle > 0 || angle < -180;
        foreach (var enemy in enemies.Select(x => x.GetComponent<Enemy>()))
        {
            Debug.Log("hit");
            GameManager.Instance.ProceedDamage(this, enemy, isStrongAttack ? StrongDamage : SimpleDamage);
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
        Debug.Log("Player died");
        Destroy(gameObject);
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