using System;
using UnityEngine;

public abstract class SingletonScriptableObject<T, CT> : ScriptableObjectBehavior where T : SingletonScriptableObject<T, CT> where CT : ICreationMethod
{
    public static T Instance
    {
        get
        {
            if (_inst == null)
            {
                CT t = (CT)Activator.CreateInstance(typeof(CT));

                switch (t)
                {
                    case ICreationMethodLocated ctl:
                        var r = Resources.LoadAll<T>("");
                        if (r.Length <= 0)
                        {
                            Debug.LogError($"No {typeof(T)} Manager Found in Resources/Manager");
                        }
                        else
                        {
                            _inst = r[0];
                            _inst.isSetuped = false;
                        }
                        break;
                    case ICreationMethodRealtime ctr:
                        _inst = ScriptableObject.CreateInstance<T>();
                        break;
                }

                _inst.Setup();

            }
            return _inst;
        }
    }
    static T _inst;
    bool isSetuped = false;

    public void Setup()
    {
        if (isSetuped)
            return;
        _inst = (T)this;
        OnSetup();
        isSetuped = true;
    }

    protected virtual void OnSetup() { }

    public sealed override void Initialize()
    {
        isSetuped = false;
        Setup();
    }

}

public interface ICreationMethod { }

public class ICreationMethodRealtime : ICreationMethod { }

public class ICreationMethodLocated : ICreationMethod { }