
using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Unity.Collections;

namespace F.Cards
{
    public class CoinCardVisual : CardVisual
    {

        private CrossDecksController crossDecksController;

        public override void BeginDrag(Card card)
        {
            base.BeginDrag(card);
            crossDecksController.OnCardBeginDrag(this);
        }

        public override void EndDrag(Card card)
        {
            base.EndDrag(card);
            crossDecksController.OnCardEndDrag();
        }

        public override void ReInitialize()
        {
            base.ReInitialize();
        }

        public override void Initialize(Card target, int index = 0)
        {
            crossDecksController = FindObjectOfType<CrossDecksController>();
        
            base.Initialize(target, index);

            material = new Material(cardImage.material);
            cardImage.material = material;
            
            materialShadow = new Material(shadowCanvas.shadowImage.material);
            shadowCanvas.shadowImage.material = materialShadow;
        }

        public override void Select(Card card, bool state)
        {
            base.Select(card, state);
        }

        public override void Swap(float dir = 1)
        {
            base.Swap(dir);
        }


        public override void PointerEnter(Card card)
        {
            base.PointerEnter(card);
        }

        public override void PointerExit(Card card)
        {
            base.PointerExit(card);
        }

        public override void PointerUp(Card card, bool longPress)
        {
            base.PointerUp(card, longPress);
        }

        public override void PointerDown(Card card)
        {
            base.PointerDown(card);
        }

        public IEnumerator DamageFlash(float flashTime){
            material.SetFloat("_FlashAmount", 1);

            WaitForEndOfFrame w = new WaitForEndOfFrame();
            
            float t = 0, orig = material.GetFloat("_FlashAmount");

            while (t < flashTime) {
                yield return w;
                t += Time.deltaTime;
                    
                material.SetFloat("_FlashAmount", ( 1 - t / flashTime) * orig);
            }


            material.SetFloat("_FlashAmount", 0);
        }



        private Material material;
        private Material materialShadow;

    }   


}
