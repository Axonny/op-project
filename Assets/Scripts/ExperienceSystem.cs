using Interfaces;
using UnityEngine;

public class ExperienceSystem : Singleton<ExperienceSystem>
{
    private float levelCoefficient = 0.2f;

    private void Awake()
    {
    }

    public void CalculateExperience(IUnit killer, IUnit killed)
    {
        var additionalExperience =
            Mathf.CeilToInt(100 * killed.Level * levelCoefficient * Mathf.Max(0, killed.Level - killer.Level + 5));
        killer.AddExperience(additionalExperience);
    }
}