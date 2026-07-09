namespace Warzone.Combat
{
    public sealed class BuildingFireResult
    {
        public BuildingFireResult(bool canFire, int? blockingBuildingId = null, bool allowThroughBuildingBlockers = false, float targetDamageMultiplier = 1f)
        {
            CanFire = canFire;
            BlockingBuildingId = blockingBuildingId;
            AllowThroughBuildingBlockers = allowThroughBuildingBlockers;
            TargetDamageMultiplier = targetDamageMultiplier;
        }

        public bool CanFire { get; private set; }
        public int? BlockingBuildingId { get; private set; }
        public bool AllowThroughBuildingBlockers { get; private set; }
        public float TargetDamageMultiplier { get; private set; }
    }
}
