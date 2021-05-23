using System;
using System.Linq;
using DialogueSystem;
using Interfaces;
using UnityEngine;

namespace PlayerScripts
{
    public class Player : Singleton<Player>, IPlayer
    {
        internal Characteristic[] _characteristics =
        {
            new Characteristic(10),
            new Characteristic(10),
            new Characteristic(10),
            new Characteristic(10),
            new Characteristic(10),
        };

        internal Characteristic[] _characteristics_delta =
        {
            new Characteristic(0),
            new Characteristic(0),
            new Characteristic(0),
            new Characteristic(0),
            new Characteristic(0),
        };

        internal int FreeSkillPoints;
        internal int skillPointsPerLevel = 5;


        private int experience;
        private int level;

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

        internal int Strength => _characteristics[0].Value + _characteristics_delta[0].Value;
        internal int Vitality => _characteristics[1].Value + _characteristics_delta[1].Value;
        internal int Agility => _characteristics[2].Value + _characteristics_delta[2].Value;
        internal int Intelligence => _characteristics[3].Value + _characteristics_delta[3].Value;
        internal int Wisdom => _characteristics[4].Value + _characteristics_delta[4].Value;
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
                UISystem.Instance.panelUIContainer.characteristic[i].text =
                    (_characteristics[i].Value + _characteristics_delta[i].Value).ToString();
            }

            UISystem.Instance.panelUIContainer.lvlInfo.text = level.ToString();
            UISystem.Instance.panelUIContainer.currentExperience.text = experience.ToString();
            UISystem.Instance.panelUIContainer.needExperience.text = NeedExperienceCurrent.ToString();
            UISystem.Instance.panelUIContainer.freeSkillPoints.text = FreeSkillPoints.ToString();
            UISystem.Instance.panelUIContainer.simpleAttackDamage.text = SimpleDamage.Size.ToString();
            UISystem.Instance.panelUIContainer.strongAttackDamage.text = StrongDamage.Size.ToString();
            UISystem.Instance.panelUIContainer.magickAttackDamage.text = MagickDamage.Size.ToString();
            UISystem.Instance.panelUIContainer.maxHealth.text = MaxHealth.ToString();
            UISystem.Instance.panelUIContainer.maxMana.text = MaxMana.ToString();
            UISystem.Instance.panelUIContainer.manaRestore.text = (2 * ManaRestore).ToString();
            UISystem.Instance.panelUIContainer.movementSpeed.text = MovementSpeed.ToString();
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
            else if (!isStrongAttack && Time.time - lastTimeAttack < comboAttackDuration)
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
                FreeSkillPoints += skillPointsPerLevel;
            }

            Experience = newExperience;
            Debug.Log("Exp: " + Experience + " \\ " + NeedExperienceCurrent);
            Debug.Log("Level: " + Level);
        }

        public void GetDamage(Damage damageGet, IUnit attacker)
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

        private void Move(Vector2 inputMovement)
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
            if (FreeSkillPoints == 0)
            {
                return;
            }

            _characteristics_delta[index].Value++;
            FreeSkillPoints--;
            UpdateCharacteristicPanel();
        }

        public void DecreaseCharacteristic(int index)
        {
            if (_characteristics_delta[index].Value == 0)
            {
                return;
            }

            _characteristics_delta[index].Value--;
            FreeSkillPoints++;
            UpdateCharacteristicPanel();
        }

        public void SaveCharacteristics()
        {
            var magicUnit = GetComponent<MagicUnit>();
            var startCoefHp = health * 1.0f / (vitalityToHealthModifier * _characteristics[1].Value);
            var startCoefMp = magicUnit.mana * 1.0f / (wisdomToManaModifier * _characteristics[4].Value);
            for (int i = 0; i < _characteristics.Length; i++)
            {
                _characteristics[i].Value += _characteristics_delta[i].Value;
                _characteristics_delta[i].Value = 0;
            }
            UpdateCharacteristicPanel();
            Health = (int) (MaxHealth * startCoefHp);
            magicUnit.Mana = (int) (startCoefMp * MaxMana);
        }

        public void ResetCharacteristics()
        {
            for (int i = 0; i < _characteristics_delta.Length; i++)
            {
                FreeSkillPoints += _characteristics_delta[i].Value;
                _characteristics_delta[i].Value = 0;
            }
            UpdateCharacteristicPanel();
        }

    }
}