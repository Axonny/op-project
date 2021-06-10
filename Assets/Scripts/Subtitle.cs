using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Subtitle : MonoBehaviour
{
    public List<SubtitleElement> subtitles;
    public string subtitleAfterTabWindow;
    public Text message;
    public float timeShow;

    private int current = -1;
    private Coroutine subtitleCoroutine;

    private void Start()
    {
        ShowSubtitles(0);
    }


    public void ShowSubtitles(int next)
    {
        if (current < next)
        {
            current = next;
            if (subtitleCoroutine != null)
                StopCoroutine(subtitleCoroutine);
            subtitleCoroutine = StartCoroutine(Subtitles());
        }
    }

    public void StartLearnUpgradePlayer()
    {
        StartCoroutine(TabWindow());
    }

    private IEnumerator Subtitles()
    {
        foreach (var sentence in subtitles[current].sentences)
        {
            message.text = "";
            foreach (var letter in sentence.ToCharArray())
            {
                message.text += letter;
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(timeShow);
        }
        message.text = "";
        subtitleCoroutine = null;
    }

    private IEnumerator TabWindow()
    {
        var window = UISystem.Instance.characteristicPanel;
        while (!window.activeSelf)
        {
            yield return null;
        }
        while (window.activeSelf)
        {
            yield return null;
        }

        message.text = subtitleAfterTabWindow;
    }
}

[System.Serializable]
public class SubtitleElement
{
    public string[] sentences;
}
