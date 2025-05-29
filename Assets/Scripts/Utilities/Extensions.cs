using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace LostOasis
{
    public static class Extensions
    {
        public static Vector3 XZ(this Vector3 vector)
        {
            return new(vector.x, 0, vector.z);
        }

        /// <summary>
        /// Turn vector3 to voxel acceptable vectorint
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3Int VX(this Vector3 vector) { return Vector3Int.FloorToInt(vector); }

        public static Vector4 Vector4(this Vector3Int vector) { return new(vector.x, vector.y, vector.z, 0.0f); }
        public static Vector3Int Vec3Int(this Vector2Int vector) { return new(vector.x, 0, vector.y); }
        public static Vector3 Vec3(this Vector2 vector) { return new(vector.x, 0, vector.y); }
        public static string MegaTrim(this string value) => string.IsNullOrEmpty(value) ? "" : value.Replace("_", "").Replace(" ", "").ToLower();

        public static string FirstUpper(this string value)
        {
            if (!String.IsNullOrEmpty(value))
                value = Char.ToUpper(value[0]) + value.Substring(1);
            return value;
        }

        public static void SetLayerAllChildren(this GameObject gameObject, int newLayer)
        {
            foreach (Transform child in gameObject.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = newLayer;
            }
        }

        public static T Randomise<T>(this List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            System.Random random = new (DateTime.Now.Millisecond ^ DateTime.Now.Second);

            return list[random.Next(0, list.Count)];
        }
        public static void Shuffle<T>(this List<T> list)
        {
            System.Random rng = new(DateTime.Now.Millisecond ^ DateTime.Now.Second);
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static float Next(this System.Random rand, float min, float max)
        {
            int mult = 10000;
            return rand.Next((int)min * mult, (int)max * mult) / (float)mult;
        }
    }
}