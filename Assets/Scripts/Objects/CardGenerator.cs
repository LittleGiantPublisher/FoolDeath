using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using F.UI;

namespace F.Cards
{
    public class CardGenerator : MonoBehaviour
    {

        private void Awake()
        {
            
            InitCardPool();
        }

        public void InitCardPool()
        {
            if (profile != null && profile.cardPool != null)
            {
                cardPool = new List<CardScriptable>(profile.cardPool);
                takenCards = new List<CardScriptable>();
            }
            else
            {
                Debug.LogError("Profile or cardPool is null. Please assign a valid CardPool profile.");
            }
        }

        public List<CardScriptable> GetRandom(int num)
        {
            if (cardPool == null)
            {
                return new List<CardScriptable>();
            }

            return GetRandom(num, cardPool);
        }

        public List<CardScriptable> GetRandom(int num, List<CardScriptable> pool, List<CardScriptable> excludeCards = null)
        {
            if (pool == null || pool.Count == 0 || num <= 0)
            {
                return new List<CardScriptable>();
            }

            List<CardScriptable> filteredPool = new List<CardScriptable>(pool);
            if (excludeCards != null && excludeCards.Count > 0)
            {
                filteredPool = filteredPool.Except(excludeCards).ToList();
            }

            num = Mathf.Min(num, filteredPool.Count);

            List<CardScriptable> randomCards = filteredPool.OrderBy(x => UnityEngine.Random.value).Take(num).ToList();

            foreach (var card in randomCards)
            {
                pool.Remove(card);
                takenCards.Add(card);
            }

            return randomCards;
        }


        public void AddToPool(List<CardScriptable> cards)
        {
            cardPool.AddRange(cards);
        }

        public void RemoveFromPool(CardScriptable card)
        {
            cardPool.Remove(card);
            takenCards.Add(card);
        }

        public void Reset()
        {
            cardPool.AddRange(takenCards);
            takenCards.Clear();
        }

        [SerializeField]
        private CardPool profile;

        public List<CardScriptable> cardPool;
        private List<CardScriptable> takenCards;
    }
}
