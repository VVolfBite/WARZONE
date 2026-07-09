using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleExtractionResult
    {
        public BattleExtractionResult(IReadOnlyList<BattleEntityId> extractedMemberIds, int survivingMemberCount)
        {
            ExtractedMemberIds = extractedMemberIds;
            SurvivingMemberCount = survivingMemberCount;
        }

        public IReadOnlyList<BattleEntityId> ExtractedMemberIds { get; private set; }
        public int SurvivingMemberCount { get; private set; }
    }
}
