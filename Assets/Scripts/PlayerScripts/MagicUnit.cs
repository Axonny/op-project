using System;
using UnityEngine;

namespace PlayerScripts
{
    public class MagicUnit : MonoBehaviour
    {
        public int mana;
        internal int costManaBase = 10;

        public int CostMana =>
            (int) (costManaBase + Math.Max(0, Player.Instance.Intelligence - 15) * 1.5f);

        public GameObject magicAttackPrefab;
        [SerializeField] private float attackDuration;
        [SerializeField] private float manaRestoreDuration;

        [SerializeField] Camera mainCamera;
        [SerializeField] private Transform fromPoint;

        private InputMaster input;
        private float lastTimeAttack;
        private float lastTimeRestoreMana;
        internal int MaxMana => Player.Instance.MaxMana;

        public int Mana
        {
            get => mana;
            set
            {
                mana = value;
                if (mana > MaxMana)
                    mana = MaxMana;
                UISystem.Instance.manaBar.value = mana * 1.0f / MaxMana * 100;
            }
        }


        private int ManaRestore => Player.Instance.ManaRestore;

        private void Awake()
        {
            input = InputSystem.Instance.Input;
            input.Player.MagicShot.performed += context => Attack();
            mana = MaxMana;
        }

        private void Update()
        {
            if (Time.time - lastTimeRestoreMana > manaRestoreDuration)
            {
                Mana += ManaRestore;
                lastTimeRestoreMana = Time.time;
            }
        }

        private void Attack()
        {
            if (Time.time - lastTimeAttack < attackDuration || Mana < CostMana)
                return;
            lastTimeAttack = Time.time;
            var position = fromPoint.position;
            var rotation = Quaternion.Euler(0, 0, Physics.GetAngleToMouse(mainCamera, position));
            var magic = Instantiate(magicAttackPrefab, position, rotation).GetComponent<MagicSpell>();
            magic.damage = Player.Instance.MagickDamage;
            Mana -= CostMana;
        }
    }
}