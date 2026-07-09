namespace Warzone.Combat
{
    public sealed class BuildingVisibilityResult
    {
        public BuildingVisibilityResult(bool hasVisibility, int? blockingBuildingId = null, bool allowThroughBuildingBlockers = false)
        {
            HasVisibility = hasVisibility;
            BlockingBuildingId = blockingBuildingId;
            AllowThroughBuildingBlockers = allowThroughBuildingBlockers;
        }

        public bool HasVisibility { get; private set; }
        public int? BlockingBuildingId { get; private set; }
        public bool AllowThroughBuildingBlockers { get; private set; }
    }
}
