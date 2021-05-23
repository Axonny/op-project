using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using Interfaces;
using PlayerScripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

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

    internal Characteristic[] _characteristics_delta =
    {
        new Characteristic("strength", 0),
        new Characteristic("vitality", 0),
        new Characteristic("agility", 0),
        new Characteristic("intelligence", 0),
        new Characteristic("wisdom", 0),
    };

    internal int freeSkillPoints = 0;
    internal int skillPointsPerLevel = 5;


    private int experience = 0;
    internal int level;

    internal int NeedExperienceCurrent => 100 + 50 * Level;

    internal int strongAttackModifier = 2;
    internal int strengthToDamageModifier = 1;
    internal int vitalityToHealthModifier = 10;
    internal int agilityToSpeedModifier = 1;
    internal int intelligenceToDamageModifier = 1;
    internal int wisdomToManaModifier = 10;
    internal float wisdomToManaRestoreModifier = 0.2f;

    private int health;
    [SerializeField] private float attackDuration;
    [SerializeField] private float comboAttackDuration;

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

    internal int Strength => _characteristics[0].value + _characteristics_delta[0].value;
    internal int Vitality => _characteristics[1].value + _characteristics_delta[1].value;
    internal int Agility => _characteristics[2].value + _characteristics_delta[2].value;
    internal int Intelligence => _characteristics[3].value + _characteristics_delta[3].value;
    internal int Wisdom => _characteristics[4].value + _characteristics_delta[4].value;
    public Damage SimpleDamage => new Damage(Strength * strengthToDamageModifier, DamageType.Physic);

    public Damage StrongDamage => new Damage(SimpleDamage.Size * strongAttackModifier, DamageType.Physic);

    public Damage MagickDamage =>
        new Damage(Intelligence * intelligenceToDamageModifier, DamageType.Magic);

    internal int MaxHealth => vitalityToHealthModifier * Vitality;
    internal int MaxMana => Wisdom * wisdomToManaModifier;

    internal int ManaRestore =>
        (int) (1 + (Math.Max(0, Wisdom - 10) * wisdomToManaRestoreModifier));

    internal int MovementSpeed => Agility * agilityToSpeedModifier;

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
        UpdateCharacteristicPanel();
        playerSave.LoadData();
    }

    internal
        void UpdateCharacteristicPanel()
    {
        for (int i = 0; i < _characteristics.Length; i++)
        {
            UISystem.Instance.PanelUIContainer.characteristic[i].text =
                (_characteristics[i].value + _characteristics_delta[i].value).ToString();
        }

        UISystem.Instance.PanelUIContainer.lvlInfo.text = level.ToString();
        UISystem.Instance.PanelUIContainer.currentExperience.text = experience.ToString();
        UISystem.Instance.PanelUIContainer.needExperience.text = NeedExperienceCurrent.ToString();
        UISystem.Instance.PanelUIContainer.freeSkillPoints.text = freeSkillPoints.ToString();
        UISystem.Instance.PanelUIContainer.simpleAttackDamage.text = SimpleDamage.Size.ToString();
        UISystem.Instance.PanelUIContainer.strongAttackDamage.text = StrongDamage.Size.ToString();
        UISystem.Instance.PanelUIContainer.magickAttackDamage.text = MagickDamage.Size.ToString();
        UISystem.Instance.PanelUIContainer.maxHealth.text = MaxHealth.ToString();
        UISystem.Instance.PanelUIContainer.maxMana.text = MaxMana.ToString();
        UISystem.Instance.PanelUIContainer.manaRestore.text = (2 * ManaRestore).ToString();
        UISystem.Instance.PanelUIContainer.movementSpeed.text = MovementSpeed.ToString();
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = movement * MovementSpeed;

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
            freeSkillPoints += skillPointsPerLevel;
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

    public void IncreaseCharacteristic(int index)
    {
        if (freeSkillPoints == 0)
        {
            return;
        }

        _characteristics_delta[index].value++;
        freeSkillPoints--;
        UpdateCharacteristicPanel();
    }

    public void DecreaseCharacteristic(int index)
    {
        if (_characteristics_delta[index].value == 0)
        {
            return;
        }

        _characteristics_delta[index].value--;
        freeSkillPoints++;
        UpdateCharacteristicPanel();
    }

    public void SaveCharacteristics()
    {
        var magicUnit = GetComponent<MagicUnit>();
        var startCoefHp = health * 1.0f / (vitalityToHealthModifier * _characteristics[1].value);
        var startCoefMp = magicUnit.mana * 1.0f / (wisdomToManaModifier * _characteristics[4].value);
        for (int i = 0; i < _characteristics.Length; i++)
        {
            _characteristics[i].value += _characteristics_delta[i].value;
            _characteristics_delta[i].value = 0;
        }
        UpdateCharacteristicPanel();
        Health = (int) (MaxHealth * startCoefHp);
        magicUnit.Mana = (int) (startCoefMp * MaxMana);
    }

    public void ResetCharacteristics()
    {
        for (int i = 0; i < _characteristics_delta.Length; i++)
        {
            freeSkillPoints += _characteristics_delta[i].value;
            _characteristics_delta[i].value = 0;
        }
        UpdateCharacteristicPanel();
    }

}