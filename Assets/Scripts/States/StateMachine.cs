using System;
using UnityEngine;

namespace F.State
{
    public class StateMachine : MonoBehaviour
    {
        public virtual State CurrentState
        {
            get
            {
                return this._currentState;
            }
            set
            {
                this.Transition(value);
            }
        }

        public virtual T GetState<T>() where T : State
        {
            T t = base.GetComponentInChildren<T>(); 
            if (t == null)
            {
                t = base.gameObject.AddComponent<T>();
            }
            return t;
        }

        public virtual void ChangeState<T>() where T : State
        {
            this.CurrentState = this.GetState<T>();
        }

        protected virtual void Transition(State value)
        {
            if (this._currentState == value)
            {
                return;
            }
            State currentState = this._currentState;
            if (currentState != null)
            {
                currentState.Exit();
            }
            this._currentState = value;
            State currentState2 = this._currentState;
            if (currentState2 == null)
            {
                return;
            }
            currentState2.Enter();
        }

        protected State _currentState;
    }
}
