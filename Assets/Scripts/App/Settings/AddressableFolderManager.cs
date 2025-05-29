using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core
{
    public class AddressableFolderManager : MonoBehaviour
    {
        [SerializeField] private string folderLabel = "YourFolderLabel"; // Set this to your folder's label
        private Dictionary<string, AsyncOperationHandle> loadedAssets = new Dictionary<string, AsyncOperationHandle>();
        public List<string> assetKeys = new List<string>();

        async void Awake()
        {
            await GetAssetsInFolder();
        }

        // Get all asset keys/addresses in a specific folder by label
        public async Task GetAssetsInFolder()
        {
            var locationsHandle = Addressables.LoadResourceLocationsAsync(folderLabel);
            await locationsHandle.Task;

            if (locationsHandle.Status == AsyncOperationStatus.Succeeded)
            {
                assetKeys.Clear();
                foreach (var location in locationsHandle.Result)
                {
                    assetKeys.Add(location.PrimaryKey);
                    Debug.Log($"Found asset: {location.PrimaryKey}");
                }
                Debug.Log($"Total assets found: {assetKeys.Count}");
            }
            else
            {
                Debug.LogError("Failed to load resource locations for label: " + folderLabel);
            }

            Addressables.Release(locationsHandle);
        }

        // Load a specific asset by key
        public async Task<T> LoadAsset<T>(string assetKey) where T : Object
        {
            if (loadedAssets.ContainsKey(assetKey))
            {
                Debug.Log($"Asset {assetKey} already loaded");
                return loadedAssets[assetKey].Result as T;
            }

            var handle = Addressables.LoadAssetAsync<T>(assetKey);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedAssets[assetKey] = handle;
                Debug.Log($"Successfully loaded: {assetKey}");
                return handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load asset: {assetKey}");
                return null;
            }
        }

        // Load all assets in the folder
        public async Task LoadAllAssets<T>() where T : Object
        {
            foreach (string key in assetKeys)
            {
                await LoadAsset<T>(key);
            }
        }

        // Unload a specific asset
        public void UnloadAsset(string assetKey)
        {
            if (loadedAssets.ContainsKey(assetKey))
            {
                Addressables.Release(loadedAssets[assetKey]);
                loadedAssets.Remove(assetKey);
                Debug.Log($"Unloaded asset: {assetKey}");
            }
            else
            {
                Debug.LogWarning($"Asset {assetKey} is not loaded");
            }
        }

        // Unload all loaded assets
        public void UnloadAllAssets()
        {
            foreach (var kvp in loadedAssets)
            {
                Addressables.Release(kvp.Value);
                Debug.Log($"Unloaded asset: {kvp.Key}");
            }
            loadedAssets.Clear();
        }

        // Get list of all asset keys in folder
        public List<string> GetAssetKeys()
        {
            return new List<string>(assetKeys);
        }

        // Check if asset is loaded
        public bool IsAssetLoaded(string assetKey)
        {
            return loadedAssets.ContainsKey(assetKey);
        }

        void OnDestroy()
        {
            UnloadAllAssets();
        }

        // Alternative method if you want to use address strings directly instead of labels
        public async Task GetAssetsInFolderByAddress(string folderAddress)
        {
            var catalogHandle = Addressables.LoadContentCatalogAsync("your_catalog_path"); // Optional: specify catalog
            await catalogHandle.Task;

            // This approach requires you to know the specific addresses
            // You would typically set up your addressables with a naming convention like:
            // "Folder/SubFolder/AssetName"

            // Example addresses you might look for:
            string[] addresses = {
            $"{folderAddress}/Asset1",
            $"{folderAddress}/Asset2",
            // etc...
        };

            assetKeys.Clear();
            foreach (string address in addresses)
            {
                var locationHandle = Addressables.LoadResourceLocationsAsync(address);
                await locationHandle.Task;

                if (locationHandle.Status == AsyncOperationStatus.Succeeded && locationHandle.Result.Count > 0)
                {
                    assetKeys.Add(address);
                    Debug.Log($"Found asset at address: {address}");
                }

                Addressables.Release(locationHandle);
            }
        }
    }
}