using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Eric.ModuleSystem
{
        public abstract class ModuleOwner : MonoBehaviour
        {
                private List<IModule> _modules = new();

                protected virtual void Awake()
                {
                        _modules = GetComponentsInChildren<MonoBehaviour>(true)
                                .OfType<IModule>()
                                .ToList();

                        ModuleInit();
                        AfterModulesInit();
                }

                protected virtual void ModuleInit()
                {
                        foreach (IModule module in _modules)
                        {
                                module.Init(this);
                        }
                }

                protected virtual void AfterModulesInit()
                {
                        foreach (IModule module in _modules)
                        {
                                module.AfterInit();
                        }
                }

                public T GetModule<T>() where T : class
                {
                        return _modules
                                .OfType<T>()
                                .FirstOrDefault();
                }

                public IEnumerable<T> GetModules<T>() where T : class
                {
                        return _modules
                                .OfType<T>();
                }
        }
}