using System.Collections.Generic;
using UnityEngine;
using Warzone.Combat;

namespace Warzone.Adapters
{
    public sealed class SandboxHudSnapshot
    {
        public int ActiveWaveIndex { get; set; }
        public int TotalWaveCount { get; set; }
        public string ObjectiveText { get; set; }
        public string NotificationText { get; set; }
        public string DebugText { get; set; }
        public string TeamText { get; set; }
        public string SpeedText { get; set; }
        public bool IsPaused { get; set; }
        public List<SandboxMinimapDot> MinimapDots { get; } = new List<SandboxMinimapDot>();
        public List<SandboxTeamSlotSnapshot> TeamSlots { get; } = new List<SandboxTeamSlotSnapshot>();
    }

    public struct SandboxMinimapDot
    {
        public Vector2 NormalizedPosition;
        public Color Color;
        public bool IsSelected;
    }

    public struct SandboxTeamSlotSnapshot
    {
        public int SlotIndex;
        public int BoundCount;
        public bool IsActive;
    }
}
