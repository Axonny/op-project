using Interfaces;
using PlayerScripts;
using UnityEngine;

public class MagicSpell : MonoBehaviour
{
    public float speed = 20;
    public LayerMask enemyMask;
    public LayerMask wallMask;
    public Damage damage = new Damage(10, DamageType.Magic);
    
    private bool isHit;

    private void Awake()
    {
        
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!isHit && ((1 << other.gameObject.layer) & enemyMask) != 0)
        {
            var enemy = other.gameObject.GetComponent<Enemy>();
            var player = Player.Instance;
            GameManager.Instance.ProceedDamage(player, enemy, damage);
            DestroyMagic();
        }

        if (!isHit && ((1 << other.gameObject.layer) & wallMask) != 0)
        {
            DestroyMagic();
        }
    }

    private void DestroyMagic()
    {
        isHit = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Destroy(gameObject, 0.1f);
    }
}
