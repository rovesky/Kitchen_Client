using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BSingleton<T> where T : class, new()
{
    protected BSingleton() { }

    class SingletonCreator
    {
        static SingletonCreator() { }
        internal static readonly T instance = new T();
    }

    public static T Instance
    {
        get { return SingletonCreator.instance; }
    }
}