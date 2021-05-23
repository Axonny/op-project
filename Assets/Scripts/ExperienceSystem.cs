using Interfaces;
using UnityEngine;

public class ExperienceSystem : Singleton<ExperienceSystem>
{

    public void CalculateExperience(IUnit killer, IUnit killed)
    {
        var coef = 1.0f;
       
        if (killed.Level >= killer.Level)
        {
            coef = 1 + (killed.Level - killer.Level * 1.0f) / killer.Level;
        }
        else
        {
            coef = Mathf.Lerp(0, 1, 1.0f + (killed.Level - killer.Level * 1.0f) / killer.Level);
        }

        var additionalExperience = Mathf.CeilToInt(killed.Experience * coef);
        killer.AddExperience(additionalExperience);
    }
}