using System;
using Key.Scripts.ASatellite.Modules;
using UnityEngine;

namespace Key.Scripts.ASatellite {
    public abstract class AbstractASatellite : ModuleOwner {
        public event Action<float> OnTick;

        protected virtual void Update() {
            OnTick?.Invoke(Time.deltaTime);
        }

        public virtual void Deploy(Transform orbitCenter) {
            MovementModule movementModule =
                GetModule<MovementModule>();

            if (movementModule == null) {
                Debug.LogError(
                    $"{name}에 MovementModule이 없습니다.",
                    this
                );

                return;
            }

            movementModule.SetCenter(orbitCenter);

            AttackModule attackModule =
                GetModule<AttackModule>();

            attackModule?.Activate();
        }
    }
}