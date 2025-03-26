using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju.Entity
{
    /// <summary>
    /// Some entity states
    /// </summary>
    public enum StateKind
    {
        // Define if entity is controlled by third party
        MovementOverrided,

        // Prevent any movement
        MovementLocked,

        // Prevent the usage of any ability
        AbilityLocked,

        // Define if an entity can take damage or not
        Invulnerable,

        // Define if entity is protected (ie shield...)
        Protected,

        // Define if entity is grounded
        Grounded,
    }

    public class State<T>
    {
        public UnityEvent<T, T> onChange;

        private T    _state;
        private bool _locked;
        private Guid _guid;

        public State(T p_init)
        {
            onChange = new();

            _locked = false;
            _state  = p_init;
            _guid   = Guid.Empty;
        }

        /// <summary>
        /// Return the state (see bool operator).
        /// </summary>
        /// <returns>The state.</returns>
        public T Get()
        {
            return _state;
        }

        /// <summary>
        /// Try to set the current state if not locked.
        /// </summary>
        /// <param name="p_state">The new state.</param>
        public void Set(T p_state)
        {
            if (!_locked && !EqualityComparer<T>.Default.Equals(_state, p_state))
            {
                var prev_state = _state;
                _state = p_state;
                onChange.Invoke(prev_state, _state);
            }
        }

        /// <summary>
        /// Lock the current state to prevent alteration.
        /// </summary>
        /// <returns>The lock key used to unlock the state.</returns>
        public Guid Lock()
        {
            if (!_locked)
            {
                _locked = true;
                return (_guid = Guid.NewGuid());
            }
            else
            {
                Debug.LogWarning("Trying to lock an already locked state! (see ForceLock if this behaviour is exepected)");
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Force lock the current state (even if that state is already locked).<br/>
        /// Be carefull with this one, because it will override last lock key.
        /// </summary>
        /// <returns>The lock key used to unlock the state.</returns>
        public Guid ForceLock()
        {
            _locked = true;
            return (_guid = Guid.NewGuid());
        }

        /// <summary>
        /// Unlock a state to allow alteration.
        /// </summary>
        /// <param name="p_guid">The key.</param>
        public void Unlock(Guid p_guid)
        {
            if (_locked)
            {
                if (_guid != Guid.Empty && _guid == p_guid)
                {
                    _locked = false;
                    _guid   = Guid.Empty;
                }
                else
                {
                    Debug.LogWarning("Trying to unlock a state with the wrong key!");
                }
            }
            else
            {
                Debug.LogWarning("Trying to unlock an already unlocked state!");
            }
        }

        /// <summary>
        /// Check if lock key is valid with the current state.
        /// </summary>
        /// <param name="p_guid">The key.</param>
        /// <returns>The validity of the given key.</returns>
        public bool IsKeyValid(Guid p_guid)
        {
            return p_guid == _guid;
        }

        public static implicit operator T (State<T> p_state)
        {
            return p_state._state;
        }
    }
}
