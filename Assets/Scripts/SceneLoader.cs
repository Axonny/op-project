using PlayerScripts;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public LevelManager levelManager;

    public void LoadSceneFromScriptableObject()
    {
        LoadScene(levelManager.NextLevel);
    }

    public void SetLevelManagerIndex(int index)
    {
        levelManager.index = index;
        Player.Instance.CurrentDialogue = index;
    }

    public void LoadScene(string nameScene)
    {
        Player.Instance.playerSave.SaveData();
        LoadSceneAsync(nameScene, true);
    }

    public void LoadSceneWithoutSaving(string nameScene)
    {
        LoadSceneAsync(nameScene);
    }

    private void LoadSceneAsync(string nameScene, bool isFade = false)
    {
        InputSystem.Instance.Input.Player.Disable();
        Time.timeScale = 1f;
        if (isFade)
        {
            UISystem.Instance.FadeIn(false);
            UISystem.Instance.ShowLoadIcon(100);
        }
        SceneManager.LoadSceneAsync(nameScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}