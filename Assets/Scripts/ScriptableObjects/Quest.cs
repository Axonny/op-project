using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        public new string name;
        public string description;
        [Range(0, 100)]
        public double progress;

        public Award award;
    }

    [System.Serializable]
    public class Award
    {
        public int experience;
        public int gold;
        // public Items[] items;
    }
}