using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : Singleton<UISystem>
{
    public Slider healthBar;
    public Slider manaBar;
    public Text lvlInfo;
    public GameObject menuPanel;
    public GameObject deadPanel;
    public GameObject loadImage;
    public GameObject hub;
    public Image fade;
    public float timeFade;
    public float timeLoad;
    
    private InputMaster input;

    private void Start()
    {
        input = InputSystem.Instance.Input;

        input.Absolute.Escape.performed += ctx =>
        {
            if (menuPanel.activeInHierarchy)
                ResumeGame();
            else
                PauseGame();
        };
    }

    public void ResumeGame()
    {
        input.Player.Enable();
        menuPanel.SetActive(false);
        hub.SetActive(true);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        input.Player.Disable();
        menuPanel.SetActive(true);
        hub.SetActive(false);
        Time.timeScale = 0f;
    }

    public void FadeIn(bool showDead)
    {
        StartCoroutine(FadeInOut(0, 0.8f, showDead));
    }

    public void FadeOut()
    {
        deadPanel.SetActive(false);
        StartCoroutine(FadeInOut(0.8f, 0, false));
    }

    public void ShowLoadIcon()
    {
        StartCoroutine(LoadIdAnimation(timeLoad));
    }

    private IEnumerator FadeInOut(float fromAlpha, float toAlpha, bool showDead)
    {
        var time = 0f;
        var colorFrom = new Color(0, 0, 0, fromAlpha);
        var colorTo = new Color(0,0,0,toAlpha);
        while (time < timeFade)
        {
            fade.color = Color.Lerp(colorFrom, colorTo, time / timeFade);
            time += Time.deltaTime;
            yield return null;
        }
        fade.color = colorTo;
        deadPanel.SetActive(showDead);
    }

    private IEnumerator LoadIdAnimation(float time)
    {
        loadImage.SetActive(true);
        yield return new WaitForSeconds(time);
        loadImage.SetActive(false);
    }
}
