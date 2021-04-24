using UnityEngine;

public class ExperienceSystem: Singleton<ExperienceSystem>
{
    public int Level { private set; get; }
    public int Experience { private set; get; }
    private float levelCoefficient = 0.2f;
    private int needExperience = 100;
    private int NeedExperienceCurrent => Mathf.CeilToInt(Mathf.Pow(2, Level - 1) * needExperience);

    private void Awake()
    {
        Level = 1;    
        Experience = 0;
    }

    public void AddExperience(int newExperience)
    {
        while (newExperience >= NeedExperienceCurrent)
        {
            newExperience -= NeedExperienceCurrent;
            Level++;
        }

        Experience = newExperience;
        Debug.Log("Exp: " + Experience + " \\ " + NeedExperienceCurrent);
        Debug.Log("Level: " + Level);
        Debug.Log("newExp: " + newExperience);
    }

    public void CalculateExperience(Enemy enemy)
    {
        var newExperience = Mathf.CeilToInt(
            Experience + enemy.experience * levelCoefficient * Mathf.Max(0, enemy.level - Level + 5));
        AddExperience(newExperience);
    }
}