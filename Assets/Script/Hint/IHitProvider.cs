using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Hint
{
    public interface IHintProvider
    {
        Transform GetHintDropZone(Card card, List<Transform> allDropZones);
    }
}
