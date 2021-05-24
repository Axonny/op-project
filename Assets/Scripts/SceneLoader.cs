using System.Collections;
using PlayerScripts;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public LevelManager levelManager;

    public void LoadSceneFromScriptableObject()
    {
        LoadSceneWithoutSaving(levelManager.NextLevel);
    }

    public void SetLevelManagerIndex(int index)
    {
        levelManager.index = index;
        Player.Instance.CurrenDialogue = index;
    }

    public void LoadScene(string nameScene)
    {
        if (nameScene == "Lobby" && Statistics.mobsKilled < 20)
        {
            StartCoroutine(UISystem.Instance.ShowNotEnoughMobsMessage(20));
            return;
        }

        Time.timeScale = 1f;
        Player.Instance.playerSave.SaveData();
        StartCoroutine(LoadSceneAsync(nameScene, true));
        Statistics.mobsKilled = 0;
    }

    public void LoadSceneWithoutSaving(string nameScene)
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneAsync(nameScene));
    }

    private IEnumerator LoadSceneAsync(string nameScene, bool isFade = false)
    {
        if (isFade)
        {
            UISystem.Instance.FadeIn(false);
            UISystem.Instance.ShowLoadIcon(100);
        }

        var res = SceneManager.LoadSceneAsync(nameScene);
        res.allowSceneActivation = false;
        while (res.progress < 0.9f)
            yield return new WaitForSeconds(0.5f);
        res.allowSceneActivation = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}