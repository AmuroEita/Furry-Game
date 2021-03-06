﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Checks whether a Unity object exists; i.e. it hasn't been destroyed
        /// </summary>
        /// <remarks>
        /// Unlike the normal null check, this will return <c>true</c> for objects
        /// that have been merely disabled.
        /// </remarks>
        /// <param name="obj"></param>
        /// <returns><c>true</c> if the object exists, otherwise <c>false</c></returns>
        public static bool IsExists(this object obj)
        {
            if (obj == null)
                return false;
            bool exists = false;
            var o = obj as UnityEngine.Object;
            if ((object)o != null)
                try
                {
                    o.hideFlags.Equals(null);
                    exists = true;
                }
                catch { }
            return exists;
        }

        /// <summary>
        /// Gets component from another component if it has one, and adds one if it doesn't
        /// </summary>
        /// <param name="obj"></param>
        /// <returns><c>true</c> if the object exists, otherwise <c>false</c></returns>
        public static T GetOrAddComponent<T>(this Component c) where T : Component
        {
            if (c == null) throw new ArgumentNullException("The operational component is null!");
            var part = c.GetComponent<T>();
            if (part == null)
                part = c.gameObject.AddComponent<T>();

            return part;
        }

        /// <summary>
        /// Returns the square of the given number
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int Square(this int num)
        {
            return num * num;
        }

        /// <summary>
        /// Returns the square of the given number
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static float Square(this float num)
        {
            return num * num;
        }

        /// <summary>
        /// Clamps an angle between -180 and +180
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float Clamp180(this float angle)
        {
            if (angle > 180f)
                angle -= 360f;
            else if (angle < -180f)
                angle += 360f;
            return angle;
        }

        /// <summary>
        /// Clamps an euler rotation between -180 and +180
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 Clamp180(this Vector3 angle)
        {
            return new Vector3(angle.x.Clamp180(),
                angle.y.Clamp180(),
                angle.z.Clamp180());
        }

        /// <summary>
        /// Clamps an euler rotation between -180 and +180
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 Clamp180(this Vector2 angle)
        {
            return new Vector2(angle.x.Clamp180(),
                angle.y.Clamp180());
        }

        public static float RandomBetween(this Vector2 vec)
        {
            return UnityEngine.Random.Range(vec.x, vec.y);
        }

        public static int RandomBetween(this Vector2Int vec)
        {
            return UnityEngine.Random.Range(vec.x, vec.y);
        }

        public static int RandomBetweenIncluded(this Vector2Int vec)
        {
            return UnityEngine.Random.Range(vec.x, vec.y + 1);
        }

        public static T PickRandom<T>(this T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
		}

		public static T PickRandom<T>(this ICollection<T> collection)
		{
			int random = UnityEngine.Random.Range(0, collection.Count);
			foreach (var item in collection)
			{
				if (random == 0)
					return item;
				random--;
			}
			throw new Exception("This shouldn't happen");
		}

		public static float LerpBetween(this Vector2 vec, float t)
        {
            return Mathf.Lerp(vec.x, vec.y, t);
        }

		public static Vector3 AlterZ(this Vector3 vector, float z)
		{
			vector.z = z;
			return vector;
		}

		public static Vector3Int AlterZ(this Vector3Int vector, int z)
		{
			vector.z = z;
			return vector;
		}

        public static void AlterAlpha(this Color color, Action<Color> setter, float alpha)
		{
            color.a = alpha;
            setter(color);
        }

		public static Color AlterAlpha(this Color color, float alpha)
		{
			color.a = alpha;
			return color;
		}

        public static Vector3Int ToVector3Int(this Vector3 vec)
        {
            vec.x += vec.x < 0 ? -1 : 0;
            vec.y += vec.y < 0 ? -1 : 0;
            vec.z += vec.z < 0 ? -1 : 0;
            return new Vector3Int((int)vec.x, (int)vec.y, (int)vec.z);
        }

        public static void SetMagnitude(this ref Vector3 vector, float newMag)
        {
            vector = vector.normalized * newMag;
        }

        public static void SetMagnitude(this ref Vector2 vector, float newMag)
        {
            vector = vector.normalized * newMag;
        }

        public static Vector3 Rotate(this Vector3 vector, Vector3 angles)
        {
            return Quaternion.Euler(angles) * vector;
        }

        public static Vector3 Rotate(this Vector3 vector, float x, float y, float z)
        {
            return Quaternion.Euler(x, y, z) * vector;
        }

        public static Vector2 Rotate(this Vector2 vector, float angles)
        {
            var cosA = Mathf.Cos(angles * Mathf.Deg2Rad);
            var sinA = Mathf.Sin(angles * Mathf.Deg2Rad);
            var rotated = vector;

            rotated.x = cosA * vector.x - sinA * vector.y;
            rotated.y = sinA * vector.x + cosA * vector.y;

            return rotated;
        }

        public static bool IsIncluded(this Vector2 bound, float value)
        {
            return value >= bound.x && value <= bound.y;
        }

        public static bool IsIncluded(this Vector2Int bound, float value)
        {
            return value >= bound.x && value <= bound.y;
        }

        public static bool IsWithInBound(this Bounds self, Bounds other)
        {
            return other.Contains(self.max) && other.Contains(self.min);
        }

        /// <summary>
        /// Attempts to add a value to a dictionary, not replacing an existing value
        /// </summary>
        /// <returns><c>true</c>, if the value was added, <c>false</c> otherwise.</returns>
        /// <param name="dict">The dictionary</param>
        /// <param name="key">The key of the item to add</param>
        /// <param name="addValue">The value of the item to add</param>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict,
            TKey key, TValue addValue)
        {
            if (dict.ContainsKey(key))
            {
                return false;
            }
            else
            {
                dict.Add(key, addValue);
                return true;
            }
        }

        public static void Add<K, V>(this IDictionary<K, V> dictionary, PairedValue<K, V> pair)
        {
            dictionary.Add(pair.Key, pair.Value);
        }

        public static void Add<K, V>(this IDictionary<K, V> dictionary, IEnumerable<PairedValue<K, V>> pairedValues)
        {
            foreach (var pair in pairedValues)
            {
                dictionary.Add(pair);
            }
        }

        // From TOMS...
        // TODO: Figure out those 'reflection' stuffs.
        /*
		static readonly char[] dotSplit = { '.' };
		static readonly Regex indexRegex = new Regex(@"data\[(\d+)\]", RegexOptions.Compiled);

		public static object GetObject(this SerializedProperty property)
		{
			object parentObject;
			return GetObject(property, out parentObject);
		}

		public static object GetObject(this SerializedProperty property, out object parentObject)
		{
			const BindingFlags bf =
				BindingFlags.Instance |
				BindingFlags.Public |
				BindingFlags.NonPublic;
			if (property == null)
				throw new ArgumentNullException();
			object obj = property.serializedObject.targetObject;
			parentObject = null;
			var pathTokens = property.propertyPath.Split(dotSplit);
			for (int i = 0; i < pathTokens.Length; i++)
			{
				parentObject = obj;
				if (obj == null)
					return null;
				var field = obj.GetType().GetField(pathTokens[i], bf);
				if (field == null)
				{
					if (pathTokens[i] != "Array")
					{
						Debug.LogError("Unable to find field " + pathTokens[i] +
									   ". Maybe it's private? (fix this)");
						return null;
					}
					var match = indexRegex.Match(pathTokens[++i]);
					if (!match.Success)
					{
						Debug.LogError("Regex was not a match: " + pathTokens[i]);
						return null;
					}
					var index = Int32.Parse(match.Groups[1].Value);
					var list = obj as IList;
					if (list == null || index < 0 || index >= list.Count)
					{
						Debug.LogError("Unable to index: " + obj);
						return null;
					}
					obj = list[index];
				}
				else
				{
					obj = field.GetValue(obj);
				}
			}
			return obj;
		}
		*/
    }
}
