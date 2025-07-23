using System;
using UnityEngine;

namespace F.State
{
    public abstract class State : MonoBehaviour
    {
        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public float transitionTime; 
    }
}