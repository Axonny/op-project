using Interfaces;
using UnityEngine;

public class MagicSpell : MonoBehaviour
{
    public float speed = 20;
    public LayerMask enemyMask;
    public LayerMask wallMask;
    public Damage damage = new Damage(10, DamageType.Magic);

    public void SetDirection(Vector3 from, Vector3 to)
    {
        GetComponent<Rigidbody2D>().velocity = (to - from).normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(((1 << other.gameObject.layer) & enemyMask) != 0)
        {
            var enemy = other.gameObject.GetComponent<Enemy>();
            var player = Player.Instance;
            GameManager.Instance.ProceedDamage(player, enemy, damage);
            DestroyMagic();
        }

        if (((1 << other.gameObject.layer) & wallMask) != 0)
        {
            DestroyMagic();
        }
    }

    private void DestroyMagic()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Destroy(gameObject, 0.1f);
    }
}
