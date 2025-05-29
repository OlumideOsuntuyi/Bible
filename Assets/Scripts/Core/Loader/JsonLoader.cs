using Core;

using UnityEngine;
using System.Text;
using Newtonsoft.Json;

[DefaultExecutionOrder(-10)]
public class JsonLoader : Singleton<JsonLoader>
{
    [SerializeField] private TextAsset bibleJsonFile;
    [SerializeField] public JSONBible bibleData;

    public static HolyBible bible;

    private void Awake()
    {
        System.Diagnostics.Stopwatch watch = new();
        watch.Start();

        bibleData = JsonConvert.DeserializeObject<JSONBible>(bibleJsonFile.text);
        bible = new HolyBible(bibleData.metadata.name, bibleData.metadata.shortname, "EN");
        bible.Load(bibleData.verses);

        ChapterLoaderManager.Instance.Load();


        Debug.Log($"completely loaded bible {watch.ElapsedMilliseconds}ms taken.");
    }

    public static JSONBible LoadData(TextAsset asset)
    {
        var bibleData = JsonUtility.FromJson<JSONBible>(asset.text);
        
        return bibleData;
    }
}


[System.Serializable]
public class JSONBible
{
    public BibleMetaData metadata;
    public JVerse[] verses;
}

[System.Serializable]
public struct BibleMetaData
{
    public string name;
    public string shortname;
    public string module;
    public string year;
    public string desctiption;
}

[System.Serializable]
public class JVerse
{
    public string book_name;
    public int book;
    public int chapter;
    public int verse;
    public string text;
}


