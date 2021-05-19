using System;
using Interfaces;
using UnityEngine;

public class MagicUnit : MonoBehaviour
{
    public int mana;
    public int maxMana;
    public int costMana = 1;
    public GameObject magicAttackPrefab;
    [SerializeField] private float attackDuration;
    [SerializeField] private float manaRestoreDuration;
    
    [SerializeField]  Camera mainCamera;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private Transform fromPoint;
    
    private InputMaster input;
    private float lastTimeAttack;
    private float lastTimeRestoreMana;
    
    public int Mana
    {
        get => mana;
        set
        {
            mana = value;
            if (mana > maxMana)
                mana = maxMana;
            UISystem.Instance.manaBar.value = mana;
        }
    }

    private void Awake()
    {
        input = InputSystem.Instance.Input;
        input.Player.MagicShot.performed += context => Attack();
    }

    private void Update()
    {
        if (Time.time - lastTimeRestoreMana > manaRestoreDuration)
        {
            Mana++;
            lastTimeRestoreMana = Time.time;
        }
    }

    public void Attack()
    {
        if (Time.time - lastTimeAttack < attackDuration || Mana < costMana)
            return;
        lastTimeAttack = Time.time;
        var position = fromPoint.position;
        var rotation = Quaternion.Euler(0, 0, Physics.GetAngleToMouse(mainCamera, position));
        var magic = Instantiate(magicAttackPrefab, position, rotation).GetComponent<MagicSpell>();
        magic.SetDirection(rotatePoint.position, attackPoint.position);
        Mana -= costMana;
    }
}
