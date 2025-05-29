using System.Collections.Generic;
using System.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Visuals;

namespace Core
{
    public class LoadLanguages : Singleton<LoadLanguages>
    {
        public List<LanguageData> languages;
        public List<LanguageFonts> fonts;
        public string loadedID;

        public string parentFolder;
        public bool loadFoldersInFolder;

        public TMP_FontAsset defaultFont;


        private async void Awake()
        {
            if(languages.Count == 0 && loadFoldersInFolder)
            {
                var folders = SubfolderUtility.GetDirectSubfoldersInResourcesMoved(parentFolder);
                foreach(var f in folders)
                {
                    var translations = SubfolderUtility.GetFilesInResourcesFolder($"{parentFolder}/{f}");
                    string[] words = f.Split('-');
                    var lang = new LanguageData()
                    {
                        languageID = f,
                        path = $"{parentFolder}/{f}",
                        language = words[1],
                        abbreviation = words[0],
                        translations = new List<TranslationData>(),
                        fonts = new List<string>()
                    };

                    foreach (var t in translations)
                    {
                        var _t = t.Split('.');
                        var asset = await LoadAddressableAsset<TextAsset>($"{lang.path}/{_t[0]}");
                        var bible = JsonLoader.LoadData(asset);
                        var data = new TranslationData()
                        {
                            name = _t[0],
                            metaData = bible.metadata
                        };

                        lang.translations.Add(data);
                    }

                    languages.Add(lang);
                }
            }
        }

        public async static Task<T> LoadAddressableAsset<T>(string addressableKey) where T : Object
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(addressableKey);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load Addressable: {addressableKey}");
                Addressables.Release(handle);
                return null;
            }
        }

        public async static Task LoadBible(string id, string language)
        {
            System.Diagnostics.Stopwatch watch = new();
            watch.Start();

            Instance.draggable.Close();

            var text = await LoadAddressableAsset<TextAsset>(id);
            if(text == null)
            {
                Debug.Log($"Null asset");
                return;
            }
            var data = JsonLoader.LoadData(text);

            JsonLoader.Instance.bibleData = data;
            JsonLoader.bible = new HolyBible(data.metadata.name, data.metadata.shortname, language);
            JsonLoader.bible.Load(data.verses);

            Instance.loadedID = id;
            ChapterLoaderManager.Instance.Load();

            Resources.UnloadAsset(text);
            // System.GC.Collect();

            Debug.Log($"completely loaded bible {watch.ElapsedMilliseconds}ms taken.");
        }


        public RectTransform content;
        public LanguageSelector prefab;
        public DraggableRect draggable;

        public void OpenRect()
        {
            draggable.Open();
            Open();
        }

        public void Open()
        {
            MegaUtils.ClearChildren(content);
            foreach(var lang in languages)
            {
                foreach(var trans in lang.translations)
                {
                    var clone = Instantiate(prefab, content);
                    clone.Set(trans.metaData.name, lang.language, $"bibles/{lang.languageID}/{trans.name}", GetFont(lang.language));
                }
            }
        }

        public void Close()
        {
            MegaUtils.ClearChildren(content);
        }

        public TMP_FontAsset GetFont(string language)
        {
            TMP_FontAsset asset = defaultFont;
            foreach(var l in languages)
            {
                if(language == l.language)
                {
                    if (l.fonts == null || l.fonts.Count == 0) continue;
                    foreach(var f in fonts)
                    {
                        if(f.name == l.fonts[0])
                        {
                            // add addressable loading here
                            return f.font;
                        }
                    }
                }
            }

            return asset;
        }
    }

    [System.Serializable]
    public class LanguageData
    {
        public string languageID;
        public string path;
        public string language;
        public string abbreviation;
        public List<TranslationData> translations;
        public List<string> fonts;
    }

    [System.Serializable]
    public class TranslationData
    {
        public string name;
        public BibleMetaData metaData;
    }

    [System.Serializable]
    public class LanguageFonts
    {
        public string name;
        public string path;
        public TMP_FontAsset font;
    }
}