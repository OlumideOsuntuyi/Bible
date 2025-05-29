using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Core
{
    public static class SubfolderUtility
    {
        /// <summary>
        /// Get all subfolders in a specific folder inside Resources_moved (Editor only)
        /// </summary>
        public static List<string> GetSubfoldersInResourcesMoved(string folderName = "")
        {
            List<string> subfolders = new List<string>();

#if UNITY_EDITOR
            string basePath = string.IsNullOrEmpty(folderName)
                ? Path.Combine(Application.dataPath, "Resources_moved")
                : Path.Combine(Application.dataPath, "Resources_moved", folderName);

            if (Directory.Exists(basePath))
            {
                string[] directories = Directory.GetDirectories(basePath, "*", SearchOption.AllDirectories);

                foreach (string dir in directories)
                {
                    // Convert absolute path to relative path from Assets folder
                    string relativePath = "Assets" + dir.Substring(Application.dataPath.Length).Replace('\\', '/');
                    subfolders.Add(relativePath);
                    Debug.Log($"Found subfolder: {relativePath}");
                }
            }
            else
            {
                Debug.LogWarning($"Folder not found at: {basePath}");
            }
#else
        Debug.LogWarning("GetSubfoldersInResourcesMoved only works in the Unity Editor");
#endif

            return subfolders;
        }

        public static string[] GetFilesInResourcesFolder(string folderPath)
        {
            // Construct full path
            string fullPath = Path.Combine(Application.dataPath, "Resources_moved", folderPath);

            // Check if directory exists
            if (!Directory.Exists(fullPath))
            {
                Debug.LogWarning($"Directory not found: {fullPath}");
                return new string[0];
            }

            // Get all files (excluding meta files)
            string[] files = Directory.GetFiles(fullPath)
                .Where(file => !file.EndsWith(".meta"))
                .Select(Path.GetFileName)
                .ToArray();

            return files;
        }

    /// <summary>
    /// Get only direct subfolders (not nested) in Resources_moved or specific folder inside it
    /// </summary>
    public static List<string> GetDirectSubfoldersInResourcesMoved(string folderName = "")
        {
            List<string> subfolders = new List<string>();

#if UNITY_EDITOR
            string basePath = string.IsNullOrEmpty(folderName)
                ? Path.Combine(Application.dataPath, "Resources_moved")
                : Path.Combine(Application.dataPath, "Resources_moved", folderName);

            if (Directory.Exists(basePath))
            {
                string[] directories = Directory.GetDirectories(basePath, "*", SearchOption.TopDirectoryOnly);

                foreach (string dir in directories)
                {
                    string relativePath = "Assets" + dir.Substring(Application.dataPath.Length).Replace('\\', '/');
                    string folderName_local = Path.GetFileName(dir);
                    subfolders.Add(folderName_local);
                    Debug.Log($"Found direct subfolder: {folderName_local} at {relativePath}");
                }
            }
            else
            {
                Debug.LogWarning($"Folder not found at: {basePath}");
            }
#endif

            return subfolders;
        }

        /// <summary>
        /// Get subfolders with their full asset paths in Resources_moved or specific folder inside it
        /// </summary>
        public static Dictionary<string, string> GetSubfoldersWithPaths(string folderName = "")
        {
            Dictionary<string, string> folderPaths = new Dictionary<string, string>();

#if UNITY_EDITOR
            string basePath = string.IsNullOrEmpty(folderName)
                ? Path.Combine(Application.dataPath, "Resources_moved")
                : Path.Combine(Application.dataPath, "Resources_moved", folderName);

            if (Directory.Exists(basePath))
            {
                string[] directories = Directory.GetDirectories(basePath, "*", SearchOption.AllDirectories);

                foreach (string dir in directories)
                {
                    string relativePath = "Assets" + dir.Substring(Application.dataPath.Length).Replace('\\', '/');
                    string folderName_local = Path.GetFileName(dir);
                    folderPaths[folderName_local] = relativePath;
                }
            }
#endif

            return folderPaths;
        }

        /// <summary>
        /// Check if Resources_moved folder or specific folder inside it exists
        /// </summary>
        public static bool ResourcesMovedExists(string folderName = "")
        {
#if UNITY_EDITOR
            string basePath = string.IsNullOrEmpty(folderName)
                ? Path.Combine(Application.dataPath, "Resources_moved")
                : Path.Combine(Application.dataPath, "Resources_moved", folderName);
            return Directory.Exists(basePath);
#else
        return false;
#endif
        }
    }

#if UNITY_EDITOR
    [System.Serializable]
    public class SubfolderWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private List<string> subfolders = new List<string>();

        [MenuItem("Tools/Show Resources_moved Subfolders")]
        public static void ShowWindow()
        {
            GetWindow<SubfolderWindow>("Resources_moved Subfolders");
        }

        void OnGUI()
        {
            GUILayout.Label("Resources_moved Subfolders", EditorStyles.boldLabel);

            if (GUILayout.Button("Refresh All Subfolders"))
            {
                subfolders = SubfolderUtility.GetSubfoldersInResourcesMoved();
            }

            if (GUILayout.Button("Get Direct Subfolders Only"))
            {
                subfolders = SubfolderUtility.GetDirectSubfoldersInResourcesMoved();
            }

            GUILayout.Space(5);
            GUILayout.Label("Get subfolders in specific folder:", EditorStyles.miniLabel);

            if (GUILayout.Button("Example: Get subfolders in 'UI' folder"))
            {
                subfolders = SubfolderUtility.GetDirectSubfoldersInResourcesMoved("UI");
            }

            GUILayout.Space(10);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (string folder in subfolders)
            {
                EditorGUILayout.LabelField(folder);
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);
            GUILayout.Label($"Total subfolders: {subfolders.Count}");
        }
    }
#endif
}