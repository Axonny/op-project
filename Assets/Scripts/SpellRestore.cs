using PlayerScripts;
using UnityEngine;


public class SpellRestore : MonoBehaviour
{
    public int restoreHealth;
    public int restoreMana;
    public TypeRestore type;

    public enum TypeRestore
    {
        Health,
        Mana
    }

    public void PickUp()
    {
        var player = Player.Instance;
        switch (type)
        {
            case TypeRestore.Health:
                if (player.Health < player.MaxHealth)
                {
                    player.Health += restoreHealth;
                    Destroy(gameObject);
                }
                break;
            case TypeRestore.Mana:
                var magic = player.GetComponent<MagicUnit>();
                if (magic.Mana < magic.MaxMana)
                {
                    magic.Mana += restoreMana;
                    Destroy(gameObject);
                }
                break;
        }
    }
}


