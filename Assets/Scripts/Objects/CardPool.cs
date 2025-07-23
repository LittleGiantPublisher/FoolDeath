using System;
using System.Collections.Generic;
using UnityEngine;
using F.UI;

namespace F.Cards
{

	[CreateAssetMenu(fileName = "CardPool", menuName = "Cards/CardPool", order = 2)]
	public class CardPool : ScriptableObject
	{
		
		public List<CardScriptable> cardPool;
	}
}