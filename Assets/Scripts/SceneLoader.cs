using System.Collections;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public LevelManager levelManager;
    
    public void LoadSceneFromScriptableObject()
    {
        LoadSceneWithoutSaving(levelManager.levels[levelManager.index++]);
    }
    
    public void LoadScene(string nameScene)
    {
        Player.Instance.playerSave.SaveData();
        StartCoroutine(LoadSceneAsync(nameScene));
    }
    
    public void LoadSceneWithoutSaving(string nameScene)
    {
        StartCoroutine(LoadSceneAsync(nameScene));
    }

    private IEnumerator LoadSceneAsync(string nameScene)
    {
        UISystem.Instance.FadeIn(false);
        UISystem.Instance.ShowLoadIcon(100);
        var res = SceneManager.LoadSceneAsync(nameScene);
        res.allowSceneActivation = false;
        while (res.progress > 0.9f)
            yield return new WaitForSeconds(0.5f);
        res.allowSceneActivation = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
