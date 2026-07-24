using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Key.Scripts.ASatellite.Modules {
    public abstract class ModuleOwner : MonoBehaviour {
        private Dictionary<Type, IModule> _modules;

        protected virtual void Awake() {
            IModule[] foundModules =
                GetComponentsInChildren<MonoBehaviour>(true)
                    .OfType<IModule>()
                    .ToArray();

            _modules = new Dictionary<Type, IModule>();

            foreach (IModule module in foundModules) {
                Type moduleType = module.GetType();

                if (_modules.ContainsKey(moduleType)) {
                    continue;
                }

                _modules.Add(moduleType, module);
                module.Initialize(this);
            }
        }

        public T GetModule<T>() where T : class, IModule {
            if (_modules.TryGetValue(typeof(T), out IModule module)) {
                return module as T;
            }

            return _modules.Values
                .OfType<T>()
                .FirstOrDefault();
        }
    }
}