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

        public void SaveData()
        {
            var player = Player.Instance;
            level = player.Level;
            experience = player.Experience;
            health = player.Health;
            mana = player.GetComponent<MagicUnit>().Mana;
            strength = player._characteristics[0].value; 
            vitality = player._characteristics[1].value; 
            agility = player._characteristics[2].value; 
            intelligence = player._characteristics[3].value; 
            wisdom = player._characteristics[4].value;
            freeSkillPoints = player.freeSkillPoints;
        }
        
        public void LoadData()
        {
            var player = Player.Instance;
            player.Level = level;
            player.Experience = experience;
            player.Health = health;
            player.GetComponent<MagicUnit>().Mana = mana;
            mana = player.GetComponent<MagicUnit>().Mana;
            player._characteristics[0].value = strength; 
            player._characteristics[1].value = vitality; 
            player._characteristics[2].value = agility; 
            player._characteristics[3].value = intelligence; 
            player._characteristics[4].value = wisdom;
            player.freeSkillPoints = freeSkillPoints;
        }
    }
}