using UnityEngine;
using System.Collections;

namespace SpacedOut.Utils
{
    /// <summary>
    /// Manager scripts should extend this class to implement a singleton pattern
    /// </summary>
    /// <typeparam name="T">Type of class</typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T instance;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                Debug.LogError("Destroying duplicate singeton of type: " + typeof(T));
                Destroy(gameObject);
            }
        }
    }
}