using System.Collections.Generic;

using LostOasis;

using UnityEngine;

namespace Dominion
{

    [DefaultExecutionOrder(-30)]
    /// <summary>
    /// Content Resource Manager
    /// </summary>
    public class CDM : MonoBehaviour
    {
        [SerializeField] private List<Prefab> list;
        [SerializeField] private List<Objects> objectList;

        private static Dictionary<string, Prefab> prefabs;
        private static Dictionary<string, GameObject> objects;
        private void Awake()
        {
            prefabs = new();
            foreach(var l in list)
            {
                prefabs.Add(l.label.MegaTrim(), l);
            }


            objects = new();
            foreach (var l in objectList)
            {
                objects.Add(l.label.MegaTrim(), l.obj);
            }
        }

        public static T GetPrefab<T>(string label) where T : MonoBehaviour
        {
            return (T)prefabs[label.MegaTrim()].obj;
        }

        public static GameObject GameObject(string label) => objects[label.MegaTrim()];
        public static Transform Transform(string label) => GameObject(label).transform;



        [System.Serializable]
        private struct Prefab
        {
            public string label;
            public MonoBehaviour obj;
        }


        [System.Serializable]
        private struct Objects
        {
            public string label;
            public GameObject obj;
        }
    }
}