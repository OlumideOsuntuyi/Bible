using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Visuals;

namespace Core
{
    public class ChapterHistoryManager : Singleton<ChapterHistoryManager>
    {
        private static List<Reference> history;
        [SerializeField] private RectTransform content;
        [SerializeField] private HistoricalReference prefab;
        [SerializeField] private DraggableRect draggable;

        private static string PATH => Path.Combine(Application.persistentDataPath, "history.bin");

        private void Awake()
        {
            LoadSave();
        }

        private static void LoadSave()
        {
            history = FileHandler.LoadObject<List<Reference>>(PATH);
        }

        private static void Save()
        {
            FileHandler.SaveObject(history, PATH);
        }

        public void Open()
        {
            history ??= new List<Reference>();
            ListHistory();  
        }

        public void Close()
        {
            Clear();  
        }

        private void Clear()
        {
            MegaUtils.ClearChildren(content);
        }

        private void ListHistory()
        {
            Clear();

            const int MAX = 30;
            int count = Mathf.Min(MAX, history.Count);

            for (int i = 0; i < count; i++)
            {
                HistoricalReference clone = Instantiate(prefab, content);
                clone.Set(history[(history.Count - i) - 1]);
            }
        }

        public void ClosePage()
        {
            draggable.Close();
        }

        public void DeleteLast()
        {
            history.RemoveAt(history.Count - 1);
            ListHistory();
            Save();

        }

        public static void Add(Reference reference)
        {
            history ??= new List<Reference>();

            if (history.Contains(reference))
            {
                history.Remove(reference);
            }

            // add
            history.Add(reference);
            Save();
        }

    }


    [System.Serializable]
    public struct Reference
    {
        public int book;
        public int chapter;
        public int verse;
    }
}