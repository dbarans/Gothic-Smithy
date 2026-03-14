using System;
using System.Collections;
using UnityEngine;
using Smithy.Core;
using Smithy.UI;

namespace Smithy.Dialogue
{
    public enum DialogueSpeaker
    {
        Player,
        NPC
    }

    [Serializable]
    public class DialogueLine
    {
        public DialogueSpeaker speaker = DialogueSpeaker.NPC;

        [TextArea(2, 4)]
        public string text = "";

        public AudioClip voiceClip;
    }

    public class DialogueNPC : MonoBehaviour, IInteractable
    {
        [SerializeField] private string npcDisplayName;
        [SerializeField] private DialogueLine[] lines = Array.Empty<DialogueLine>();
        [SerializeField] private DialoguePanelUI dialoguePanel;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float defaultDuration = 3f;
        [SerializeField] private float pauseBetweenLines;

        public string GetPromptText() => "Rozmawiaj";

        public void Interact(InteractContext context)
        {
            if (dialoguePanel == null || lines == null || lines.Length == 0) return;
            StopAllCoroutines();
            StartCoroutine(PlaySequence());
        }

        private IEnumerator PlaySequence()
        {
            bool isPlayer0 = lines[0].speaker == DialogueSpeaker.Player;
            dialoguePanel.Show(GetSpeakerName(lines[0].speaker), isPlayer0);
            dialoguePanel.SetText(lines[0].text);
            PlayClip(lines[0].voiceClip);
            yield return new WaitForSeconds(GetDuration(lines[0]));

            for (int i = 1; i < lines.Length; i++)
            {
                yield return new WaitForSeconds(pauseBetweenLines);
                bool isPlayer = lines[i].speaker == DialogueSpeaker.Player;
                dialoguePanel.SetSpeaker(GetSpeakerName(lines[i].speaker), isPlayer);
                dialoguePanel.SetText(lines[i].text);
                PlayClip(lines[i].voiceClip);
                yield return new WaitForSeconds(GetDuration(lines[i]));
            }

            dialoguePanel.Hide();
        }

        private string GetSpeakerName(DialogueSpeaker speaker)
        {
            return speaker == DialogueSpeaker.Player ? "" : npcDisplayName;
        }

        private float GetDuration(DialogueLine line)
        {
            if (line.voiceClip != null) return Mathf.Max(line.voiceClip.length, 0.1f);
            return defaultDuration;
        }

        private void PlayClip(AudioClip clip)
        {
            if (audioSource == null || clip == null) return;
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
