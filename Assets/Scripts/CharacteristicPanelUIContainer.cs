using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacteristicPanelUIContainer
{
    [SerializeField] public Text lvlInfo;
    [SerializeField] public Text currentExperience;
    [SerializeField] public Text needExperience;
    [SerializeField] public Text[] characteristic;
    [SerializeField] public Text freeSkillPoints;
    [SerializeField] public Text simpleAttackDamage;
    [SerializeField] public Text strongAttackDamage;
    [SerializeField] public Text magickAttackDamage;
    [SerializeField] public Text maxHealth;
    [SerializeField] public Text maxMana;
    [SerializeField] public Text manaRestore;
    [SerializeField] public Text movementSpeed;
}