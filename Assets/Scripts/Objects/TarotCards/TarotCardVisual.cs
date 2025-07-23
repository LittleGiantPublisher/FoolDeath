using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Unity.Collections;
using UnityEngine.UI;

namespace F.Cards
{
    public class TarotCardVisual : CardVisual
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

            materialSuit = new Material(cardSuit.material);
            cardSuit.material = materialSuit;
        }

        public override void Select(Card card, bool state)
        {
            base.Select(card, state);
        }

        public override void Swap(float dir = 1)
        {
            base.Swap(dir);
        }


        private void SetAlpha(Graphic graphic, float alpha)
        {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }

        public override void PointerEnter(Card card)
        {
            base.PointerEnter(card);
            if (isStatic)
            {
                SetAlpha(cardImage, 0.45f);
                SetAlpha(shadowCanvas.shadowImage, 0f);
            }
        }

        public override void PointerExit(Card card)
        {
            base.PointerExit(card);
            SetAlpha(cardImage, 1.0f);
            SetAlpha(shadowCanvas.shadowImage, 0.7f);
            
        }

        public override void PointerUp(Card card, bool longPress)
        {
            base.PointerUp(card, longPress);
        }

        public override void PointerDown(Card card)
        {
            base.PointerDown(card);
        }

        public IEnumerator DamageFlash(float flashTime = -1){

            material.SetFloat("_FlashAmount", 1);

            if (flashTime <= 0f)
                flashTime = flashDuration;

            WaitForEndOfFrame w = new WaitForEndOfFrame();
            
            float t = 0, orig = material.GetFloat("_FlashAmount");

            while (t < flashTime) {
                yield return w;
                t += Time.deltaTime;
                    
                material.SetFloat("_FlashAmount", ( 1 - t / flashTime) * orig);
            }


            material.SetFloat("_FlashAmount", 0);
        }

        public void DissapearInstantly(){
            SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>(true); 
                    
            material.SetFloat("_Fade", 0);
            materialShadow.SetFloat("_Fade", 0);
            materialSuit.SetFloat("_Fade", 0);
            foreach(SpriteRenderer renderer in childRenderers) {
                renderer.material =  material;
                renderer.material.SetFloat("_Fade", 0);
            }
            
        }

        public IEnumerator Dissapear(){
            if(parentCard is TarotCard card)card.vanishSFX.Play(null);

            WaitForEndOfFrame w = new WaitForEndOfFrame();
            
            float t = 0, orig = material.GetFloat("_Fade");

            SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>(true); //gets any child sprite from enemy
            

            while (t < dissolveTime) {
                yield return w;
                t += Time.deltaTime;
                    
                material.SetFloat("_Fade", ( 1 - t / dissolveTime) * orig);
                float fade = material.GetFloat("_Fade");
                materialShadow.SetFloat("_Fade", fade);
                materialSuit.SetFloat("_Fade", fade);
                foreach(SpriteRenderer renderer in childRenderers) {
                    renderer.material =  material;
                    renderer.material.SetFloat("_Fade", fade);
                }
            }

            Destroy(parentCard.transform.parent.gameObject);
        }

        public IEnumerator Appear() {
            WaitForEndOfFrame w = new WaitForEndOfFrame();
            
            float t = 0;
            material.SetFloat("_Fade", 0);

            SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>(true); // Pega qualquer sprite filho do inimigo
            
            while (t < dissolveTime) {
                yield return w;
                t += Time.deltaTime;

                float fade = t / dissolveTime; 
                material.SetFloat("_Fade", fade);
                materialShadow.SetFloat("_Fade", fade);
                materialSuit.SetFloat("_Fade", fade);
                foreach (SpriteRenderer renderer in childRenderers) {
                    renderer.material = material;
                    renderer.material.SetFloat("_Fade", fade);
                }
            }
            
            material.SetFloat("_Fade", 1);
        }



        private Material material;
        private Material materialShadow;
        private Material materialSuit;

        [SerializeField]
        private float dissolveTime = 0.5f;

        [SerializeField]
        private float flashDuration = 0.5f;

        private Coroutine alphaTransitionCoroutine; 

    }   


}
