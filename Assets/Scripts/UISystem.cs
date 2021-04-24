using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : Singleton<UISystem>
{
    public Slider healthBar;
    public Slider experienceBar;
    public Text lvlInfo;
    private void Awake()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();
        experienceBar = GameObject.Find("ExpBar").GetComponent<Slider>();
        lvlInfo = GameObject.Find("LevelInfo").GetComponent<Text>();
    }
}
