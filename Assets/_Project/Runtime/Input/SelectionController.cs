using System;
using UnityEngine;

namespace Warzone.Runtime.Input
{
    public sealed class SelectionController : MonoBehaviour
    {
        public event Action<int> SquadSelected;

        public void SelectSquad(int squadId)
        {
            SquadSelected?.Invoke(squadId);
        }
    }
}



