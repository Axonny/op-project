using UnityEngine;

namespace PlayerScripts
{
    public class MagicUnit : MonoBehaviour
    {
        public int mana;
        public int costMana = 5;
        public GameObject magicAttackPrefab;
        [SerializeField] private float attackDuration;
        [SerializeField] private float manaRestoreDuration;

        [SerializeField] Camera mainCamera;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private Transform rotatePoint;
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
            if (Time.time - lastTimeAttack < attackDuration || Mana < costMana)
                return;
            lastTimeAttack = Time.time;
            var position = fromPoint.position;
            var rotation = Quaternion.Euler(0, 0, Physics.GetAngleToMouse(mainCamera, position));
            var magic = Instantiate(magicAttackPrefab, position, rotation).GetComponent<MagicSpell>();
            magic.damage = Player.Instance.MagickDamage;
            magic.SetDirection(rotatePoint.position, attackPoint.position);
            Mana -= costMana;
        }
    }
}