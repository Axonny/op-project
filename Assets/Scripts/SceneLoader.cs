using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public LevelManager levelManager;
    
    public void LoadSceneFromScriptableObject()
    {
        LoadScene(levelManager.levels[levelManager.index++]);
    }
    
    public void LoadScene(string nameScene)
    {
        Player.Instance.playerSave.SaveData();
        SceneManager.LoadScene(nameScene, LoadSceneMode.Single);
    }
    
    public void LoadSceneWithoutSaving(string nameScene)
    {
        SceneManager.LoadScene(nameScene, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
