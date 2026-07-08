using System;
using UnityEngine;
using Warzone.Application;
using Warzone.Combat;

namespace Warzone.Runtime.Bootstrap
{
    public sealed class BattleRuntimeHost : MonoBehaviour, IBattleRuntimeHost
    {
        public event Action<MissionStartRequest> BattleStarted;
        public event Action<BattleResult> BattleFinished;

        public void StartBattle(MissionStartRequest request)
        {
            BattleStarted?.Invoke(request);
        }

        public void FinishBattle(BattleResult result)
        {
            BattleFinished?.Invoke(result);
        }
    }
}



