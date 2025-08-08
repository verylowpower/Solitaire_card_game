using UnityEngine;
using Core;
using Rules;

namespace Managers
{
    public class RuleManager : MonoBehaviour
    {
        public static RuleManager instance;

        private IGameRule tableauRule;
        private IGameRule foundationRule;

        private void Awake()
        {
            instance = this;
            tableauRule = new TableauRule();
            foundationRule = new FoundationRule();
        }

        public bool IsValidTableauDrop(Card current, Card target)
        {
            return tableauRule.IsValidDrop(current, target);
        }

        public bool IsValidFoundationDrop(Card current, Card target)
        {
            return foundationRule.IsValidDrop(current, target);
        }

        public bool ValidateMove(Card current, Card target, string zoneType, int stackLength)
        {
            switch (zoneType)
            {
                case "Tableau":
                    return IsValidTableauDrop(current, target);
                case "Foundation":
                    if (stackLength > 1) return false; // chỉ kéo 1 lá
                    return IsValidFoundationDrop(current, target);
                default:
                    return false;
            }
        }
    }
}
