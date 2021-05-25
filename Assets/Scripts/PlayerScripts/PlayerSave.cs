using ScriptableObjects;
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
        public int strength;
        public int vitality;
        public int agility;
        public int intelligence;
        public int wisdom;
        public int freeSkillPoints;

        public void LoadCheckpoint()
        {
            var player = Player.Instance;
            health = player.Health;
            mana = player.GetComponent<MagicUnit>().Mana;
        }

        public void SaveData()
        {
            var player = Player.Instance;
            level = player.Level;
            experience = player.Experience;
            health = player.Health;
            mana = player.GetComponent<MagicUnit>().Mana;
            strength = player._characteristics[0].Value;
            vitality = player._characteristics[1].Value;
            agility = player._characteristics[2].Value;
            intelligence = player._characteristics[3].Value;
            wisdom = player._characteristics[4].Value;
            freeSkillPoints = player.FreeSkillPoints;
        }

        public void LoadData()
        {
            var player = Player.Instance;
            player.Level = level;
            player.Experience = experience;
            player.Health = health;
            player.GetComponent<MagicUnit>().Mana = mana;
            mana = player.GetComponent<MagicUnit>().Mana;
            player._characteristics[0].Value = strength;
            player._characteristics[1].Value = vitality;
            player._characteristics[2].Value = agility;
            player._characteristics[3].Value = intelligence;
            player._characteristics[4].Value = wisdom;
            player.FreeSkillPoints = freeSkillPoints;
        }

        public void ClearData()
        {
            level = 1;
            experience = 0;
            health = 50;
            mana = 50;
            strength = 10;
            vitality = 10;
            agility = 10;
            intelligence = 10;
            wisdom = 10;
            freeSkillPoints = 0;
        }
    }
}