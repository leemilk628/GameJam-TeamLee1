using System;
using UnityEngine;

namespace Eric.EventChannels
{
        public abstract class GameEvenetChannelSO<T> : ScriptableObject
        {
                public event Action<T> Raised;

                public void Raise(T value)
                {
                        Raised?.Invoke(value);
                }
        }
}