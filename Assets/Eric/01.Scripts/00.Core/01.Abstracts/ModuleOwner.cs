using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Eric.ModuleSystem
{
        public abstract class ModuleOwner : MonoBehaviour
        {
                private Dictionary<Type, IModule> _modules = new();

                protected virtual void Awake()
                {
                        _modules = GetComponentsInChildren<MonoBehaviour>(true)
                                .OfType<IModule>()
                                .ToDictionary(module => module.GetType(), module => module);

                        ModuleInit();
                        AfterModulesInit();
                }

                protected virtual void ModuleInit()
                {
                        foreach (IModule module in _modules.Values)
                        {
                                module.Init(this);
                        }
                }

                protected virtual void AfterModulesInit()
                {
                        foreach (IModule module in _modules.Values)
                        {
                                module.AfterInit();
                        }
                }

                public T GetModule<T>() where T : class
                {
                        if (_modules.TryGetValue(typeof(T), out IModule module))
                        {
                                return module as T;
                        }

                        return _modules.Values.FirstOrDefault(x => x is T) as T;
                }
        }
}