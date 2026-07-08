using UnityEngine;
using Warzone.Application;

namespace Warzone.Sandbox.Bootstrap
{
    public sealed class SandboxMissionStarter : MonoBehaviour
    {
        private IMissionFlow _missionFlow;
        private MissionStartRequest _missionStartRequest;

        public void Configure(IMissionFlow missionFlow, MissionStartRequest missionStartRequest)
        {
            _missionFlow = missionFlow;
            _missionStartRequest = missionStartRequest;
        }

        public void StartDemoMission()
        {
            _missionFlow.StartMission(_missionStartRequest);
        }
    }
}



