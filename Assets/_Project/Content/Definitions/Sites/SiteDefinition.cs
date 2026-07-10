using System.Collections.Generic;

namespace Warzone.Content.Definitions
{
    public sealed class SiteDefinition
    {
        private readonly List<string> _tags = new List<string>();

        public SiteDefinition(
            string id,
            string displayName,
            SiteType siteType,
            bool isEnterable = true,
            int baseThreatLevel = 0,
            string lootRemainingHint = null,
            string requiredMissionType = null,
            string defaultOutpostId = null)
            : this(
                id,
                displayName,
                siteType,
                isEnterable,
                baseThreatLevel,
                lootRemainingHint,
                requiredMissionType,
                5,
                0,
                0,
                true,
                null,
                defaultOutpostId)
        {
        }

        public SiteDefinition(
            string id,
            string displayName,
            SiteType siteType,
            bool isEnterable,
            int baseThreatLevel,
            string lootRemainingHint,
            string requiredMissionType,
            int maxThreatLevel,
            int initialLootRemaining,
            int resourceRichness,
            bool canBecomeOutpost,
            IReadOnlyList<string> tags,
            string defaultOutpostId = null)
        {
            Id = id;
            DisplayName = displayName;
            SiteType = siteType;
            IsEnterable = isEnterable;
            BaseThreatLevel = baseThreatLevel;
            LootRemainingHint = lootRemainingHint;
            RequiredMissionType = requiredMissionType;
            MaxThreatLevel = maxThreatLevel < 0 ? 0 : maxThreatLevel;
            InitialLootRemaining = initialLootRemaining < 0 ? 0 : initialLootRemaining;
            ResourceRichness = resourceRichness < 0 ? 0 : resourceRichness;
            CanBecomeOutpost = canBecomeOutpost;
            DefaultOutpostId = defaultOutpostId;
            SyncTags(tags);
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public SiteType SiteType { get; private set; }
        public bool IsEnterable { get; private set; }
        public int BaseThreatLevel { get; private set; }
        public string LootRemainingHint { get; private set; }
        public string RequiredMissionType { get; private set; }
        public int MaxThreatLevel { get; private set; }
        public int InitialLootRemaining { get; private set; }
        public int ResourceRichness { get; private set; }
        public bool CanBecomeOutpost { get; private set; }
        public string DefaultOutpostId { get; private set; }

        public IReadOnlyList<string> Tags
        {
            get { return _tags; }
        }

        private void SyncTags(IReadOnlyList<string> tags)
        {
            _tags.Clear();
            if (tags == null)
            {
                return;
            }

            for (int i = 0; i < tags.Count; i++)
            {
                string tag = tags[i];
                if (!string.IsNullOrEmpty(tag) && !_tags.Contains(tag))
                {
                    _tags.Add(tag);
                }
            }
        }
    }
}
