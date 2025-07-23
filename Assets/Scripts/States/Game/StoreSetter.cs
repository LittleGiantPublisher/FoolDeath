
using System;
using CameraShake;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using F;
using F.Cards;


namespace F.UI
{
	public class StoreSetter : MonoBehaviour
	{
		
		public void OnClickEmerald()
		{
            if(CombatStateStatus.Money >= 1){
                CombatStateStatus.Money -= 1;
            }
            else{
                return;
            }
            DeckVisual coinHandDeck = coinPanel.GetComponentInChildren<DeckVisual>();
                    
			coinHandDeck.AddCard(coinEmerald); 
            coinBuy.Play(null); 
		}

		public void OnClickSafira()
		{
            if(CombatStateStatus.Money >= 2){
                CombatStateStatus.Money -= 2;
            }
            else{
                return;
            }
            DeckVisual coinHandDeck = coinPanel.GetComponentInChildren<DeckVisual>();
                    
			coinHandDeck.AddCard(coinSafira);  
            coinBuy.Play(null);
		}

		public void OnClickRuby()
		{
            if(CombatStateStatus.Money >= 4){
                CombatStateStatus.Money -= 4;
            }
            else{
                return;
            }
            DeckVisual coinHandDeck = coinPanel.GetComponentInChildren<DeckVisual>();
                    
			coinHandDeck.AddCard(coinRuby);  
            coinBuy.Play(null);
		}



        public F.UI.Panel coinPanel;
        public F.Cards.CoinScriptable coinEmerald;
        public F.Cards.CoinScriptable coinSafira;
        public F.Cards.CoinScriptable coinRuby;

        [SerializeField]
		private SoundEffectSO coinBuy;
	}
}
