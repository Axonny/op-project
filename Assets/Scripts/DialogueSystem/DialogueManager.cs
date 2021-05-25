using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem.GraphData;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        public Text message;
        public GameObject dialogueWindow;
        public Transform buttonParent;
        public GameObject continueButton;
        public DialogueData dialogue;

        public bool isTalk;
        
        private Coroutine typing;
        private bool isCanceled;
        [SerializeField] private GameObject buttonPrefab;

        public void StartDialogue(DialogueData dialogueContainer)
        {
            dialogue = dialogueContainer;
            isTalk = true;

            NextSentence(dialogue.entryPoint.targetNodeGuid);
            dialogueWindow.SetActive(true);
        }

        public void CancelTyping()
        {
            isCanceled = true;
        }
        
        public void EndDialogue()
        {
            dialogueWindow.SetActive(false);
            continueButton.SetActive(false);
            isTalk = false;
        }

        private void NextSentence(string guid)
        {
            buttonParent.gameObject.SetActive(false);
            if (string.IsNullOrEmpty(guid))
            {
                EndDialogue();
                return;
            }
            var next = dialogue.Nodes.First(x => x.guid == guid);
            switch (next)
            {
                case DialogueNodeData dialogueNodeData:
                    var sentence = dialogueNodeData.dialogueText;
                    if (typing != null)
                        StopCoroutine(typing);
                    typing = StartCoroutine(TypeSentence(sentence));
                    GenerateResponseButtons(dialogueNodeData.outputPorts);
                    break;
            }
            
        }

        private void GenerateResponseButtons(List<NodeLinkData> answers)
        {
            for (var i = 0; i < buttonParent.childCount; i++)
            {
                var child = buttonParent.GetChild(i);
                Destroy(child.gameObject);
            }

            if (answers.Count == 0)
            {
                continueButton.SetActive(true);
            }
            
            foreach (var answer in answers)
            {
                var button = Instantiate(buttonPrefab, buttonParent).GetComponent<Button>();
                button.onClick.AddListener(() => { NextSentence(answer.targetNodeGuid); });
                button.GetComponentInChildren<Text>().text = answer.portName;
            }
        }
        private IEnumerator TypeSentence(string sentence)
        {
            message.text = "";
            foreach (var letter in sentence.ToCharArray())
            {
                if (isCanceled)
                    break;
                message.text += letter;
                yield return null;
            }

            message.text = sentence;
            isCanceled = false;
            typing = null;
            buttonParent.gameObject.SetActive(true);
        }
    }
}