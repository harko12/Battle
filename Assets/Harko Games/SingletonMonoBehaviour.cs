using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton monobehavior ancestor, borrowed from Apex RTS Demo
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <value>
    /// The instance.
    /// </value>
    public static T instance { get; private set; }

    /// <summary>
    /// Called by Unity when this instance awakes.
    /// </summary>
    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = (T)this;
    }
}
