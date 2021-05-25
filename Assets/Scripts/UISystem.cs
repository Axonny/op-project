using System.Collections;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UISystem : Singleton<UISystem>
{
    public Slider healthBar;
    public Slider manaBar;
    public Text lvlInfo;

    [FormerlySerializedAs("PanelUIContainer")] [SerializeField]
    public CharacteristicPanelUIContainer panelUIContainer;

    public GameObject characteristicPanel;
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
        input.Absolute.CharacteristicPanel.performed += ctx =>
        {
            if (menuPanel.activeSelf)
                return;
            if (characteristicPanel.activeInHierarchy)
            {
                Player.Instance.UpdateCharacteristicPanel();
                Player.Instance.SaveCharacteristics();
                ResumeGame(characteristicPanel);
                if (menuPanel.activeSelf)
                {
                    Time.timeScale = 0f;
                    hub.SetActive(false);
                }
            }
            else
            {
                Player.Instance.UpdateCharacteristicPanel();
                PauseGame(characteristicPanel);
            }
        };
        input.Absolute.Escape.performed += ctx =>
        {
            if (characteristicPanel.activeSelf)
            {
                characteristicPanel.SetActive(false);
                hub.SetActive(true);
                Time.timeScale = 1f;
                return;
            }
            if (menuPanel.activeInHierarchy)
            {
                ResumeGame(menuPanel);
                if (characteristicPanel.activeSelf)
                {
                    Time.timeScale = 0f;
                    hub.SetActive(false);
                }
            }

            else
            {
                PauseGame(menuPanel);
            }
        };
    }

    public void ResumeGame(GameObject panel)
    {
        input.Player.Enable();
        panel.SetActive(false);
        hub.SetActive(true);
        Time.timeScale = 1f;
    }

    public void PauseGame(GameObject panel)
    {
        input.Player.Disable();
        panel.SetActive(true);
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

    public void ShowLoadIcon(float time = 0f)
    {
        StartCoroutine(LoadIdAnimation(time < 0.01f ? timeLoad : time));
    }

    private IEnumerator FadeInOut(float fromAlpha, float toAlpha, bool showDead)
    {
        var time = 0f;
        var colorFrom = new Color(0, 0, 0, fromAlpha);
        var colorTo = new Color(0, 0, 0, toAlpha);
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