using System;
using System.Collections;
using Interfaces;
using PlayerScripts;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IEnemy
{
    private int health;
    [SerializeField] private int maxHealth = 100;
    public float deadTime;
    internal bool CanMove = true;
    internal event Action ONDead;
    public Slider hpBar;
    public bool IsDead { get; set; }
    public int level = 1;
    public int experience;
    [SerializeField] private GameObject view;
    [SerializeField] private Damage damage = new Damage(10, DamageType.Physic);
    [SerializeField] private float attackDuration;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float circleRadius;
    [SerializeField] private LayerMask playerLayer;

    private Animator animator;
    private float lastTimeAttack;
    private Player player;
    private SpriteRenderer sprite;

    private static readonly int HitAnimation = Animator.StringToHash("Hit");
    private static readonly int HealthProperty = Animator.StringToHash("Health");
    private static readonly int AttackAnimation = Animator.StringToHash("Attack");


    public int Health { 
        get => health; 
        set => health = value; 
    }

    public int Level
    {
        get => level;
        set => level = value;
    }

    public int Experience
    {
        get => experience;
        set => experience = value;
    }


    private void Awake()
    {
        health = maxHealth;
        animator = gameObject.GetComponent<Animator>();
        player = Player.Instance;
        sprite = view.GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        var vector = player.transform.position - transform.position;
        var angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg - 90f;
        rotatePoint.rotation = Quaternion.Euler(0, 0, angle);
        sprite.flipX = angle > 0 || angle < -180;
        if (Vector3.Distance(player.transform.position, attackPoint.position) < circleRadius / 2)
        {
            CanMove = false;
            if (Time.time - lastTimeAttack >= attackDuration)
                Attack();
        }
        else
        {
            CanMove = true;
        }
    }

    public void AddExperience(int experienceGet)
    {
    }

    public void GetDamage(Damage damageGet, IUnit attacker)
    {
        health -= damageGet.Size;
        animator.SetTrigger(HitAnimation);
        animator.SetInteger(HealthProperty, health);
        hpBar.value = Health * 1.0f / maxHealth * 100;
    }

    public void Attack()
    {
        lastTimeAttack = Time.time;
        animator.SetTrigger(AttackAnimation);
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        var t = 0f;
        var startPos = Vector3.zero;
        var endPos = startPos + (attackPoint.position - rotatePoint.position) / 5f;
        while (t < attackDuration)
        {
            view.transform.localPosition = Vector3.Lerp(startPos, endPos, animationCurve.Evaluate(t / attackDuration));
            t += Time.deltaTime;
            yield return null;
        }

        view.transform.localPosition = startPos;
        var playerCol = Physics.FindCollider(attackPoint.position, circleRadius, playerLayer);
        if (playerCol != null)
        {
            GameManager.Instance.ProceedDamage(this, player, damage);
        }
    }

    public void Dead()
    {
        Debug.Log("Enemy died");
        GameManager.Instance.Enemies.Remove(this);
        
        ONDead?.Invoke();
        Destroy(gameObject, deadTime);
    }
}