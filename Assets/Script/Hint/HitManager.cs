using System.Collections.Generic;
using UnityEngine;
using Core;


namespace Hint
{
    public class HintManager
    {
        public static HintManager instance;
        private IHintProvider hintProvider;

        void Awake() => instance = this;

        public HintManager(IHintProvider provider)
        {
            hintProvider = provider;
        }

        public Transform GetHint(Card selectedCard, List<Transform> allDropZones)
        {
            return hintProvider.GetHintDropZone(selectedCard, allDropZones);
        }
    }
}
