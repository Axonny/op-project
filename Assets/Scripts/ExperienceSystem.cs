using Interfaces;
using UnityEngine;

public class ExperienceSystem : Singleton<ExperienceSystem>
{

    public void CalculateExperience(IUnit killer, IUnit killed)
    {
        var coef = Mathf.Exp((killed.Level - killer.Level) * 0.33f);
       
        var additionalExperience = Mathf.CeilToInt(killed.Experience * coef);
        killer.AddExperience(additionalExperience);
    }
}