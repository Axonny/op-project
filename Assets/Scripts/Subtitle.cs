using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Subtitle : MonoBehaviour
{
    public List<SubtitleElement> subtitles;
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
}

[System.Serializable]
public class SubtitleElement
{
    public string[] sentences;
}
