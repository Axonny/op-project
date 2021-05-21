using UnityEngine;

namespace PlayerScripts
{
    [CreateAssetMenu(fileName = "Player Save", menuName = "ScriptableObjects/Player Save", order = 0)]
    public class PlayerSave : ScriptableObject
    {
        public int level;
        public int experience;
        public int health;
        public int mana;

        public void SaveData()
        {
            var player = Player.Instance;
            level = player.Level;
            experience = player.Experience;
            health = player.Health;
            mana = player.GetComponent<MagicUnit>().Mana;
        }
        
        public void LoadData()
        {
            var player = Player.Instance;
            player.Level = level;
            player.Experience = experience;
            player.Health = health;
            player.GetComponent<MagicUnit>().Mana = mana;
        }
    }
}