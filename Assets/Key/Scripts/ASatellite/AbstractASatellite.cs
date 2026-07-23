using System;
using Key.Scripts.ASatellite.Modules;
using UnityEngine;

namespace Key.Scripts.ASatellite {
    public abstract class AbstractASatellite : ModuleOwner {
        public event Action<float> OnTick;

        protected override void Awake() {
            base.Awake();
        }

        protected virtual void Update() {
            OnTick?.Invoke(Time.deltaTime);
        }
    }
}