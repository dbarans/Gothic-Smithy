using UnityEngine;
using TMPro;

namespace Smithy.UI
{
    public class DialoguePanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text npcNameText;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Color playerTextColor = Color.white;
        [SerializeField] private Color npcTextColor = Color.yellow;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        public void Show(string speakerName, bool isPlayer = false)
        {
            SetSpeaker(speakerName, isPlayer);
            if (panelRoot != null) panelRoot.SetActive(true);
        }

        public void SetSpeaker(string speakerName, bool isPlayer = false)
        {
            if (npcNameText != null) npcNameText.text = speakerName ?? "";
            if (dialogueText != null) dialogueText.color = isPlayer ? playerTextColor : npcTextColor;
        }

        public void SetText(string text)
        {
            if (dialogueText != null) dialogueText.text = text ?? "";
        }

        public void Hide()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }
    }
}
