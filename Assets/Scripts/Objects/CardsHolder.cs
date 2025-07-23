using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F.Cards{
    public class CardsHolder : MonoBehaviour
    {

        public static CardsHolder instance;

        private void Awake()
        {
            instance = this;
        }

    }
}