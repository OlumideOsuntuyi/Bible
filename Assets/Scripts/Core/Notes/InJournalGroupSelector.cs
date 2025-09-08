using Core.Notes;

using TMPro;

using UnityEngine;

namespace Visuals
{
    public class InJournalGroupSelector : MonoBehaviour
    {
        [SerializeField] private string group;
        public TMP_Text label;
        public TMP_Text amount;

        public void Set(string group, int amount)
        {
            this.group = group;
            label.text = group;
            this.amount.text = amount < 0 ? "" : MegaUtils.AddCommasToNumber(amount);
        }

        public void Open()
        {
            NoteManager.Instance.OpenSelectedNoteGroup(group);
        }
    }
}