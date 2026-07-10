using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Campaign
{
    public sealed class CampaignSiteState
    {
        private readonly List<string> _tags = new List<string>();

        public CampaignSiteState(
            string siteId,
            string displayName,
            bool isDiscovered = false,
            bool isCleared = false,
            int threatLevel = 0,
            bool searchCompleted = false,
            string lootRemainingHint = null,
            float lastVisitedTime = 0f)
            : this(
                siteId,
                displayName,
                SiteType.Outpost,
                isDiscovered,
                isCleared,
                threatLevel,
                searchCompleted,
                lootRemainingHint,
                lastVisitedTime,
                5,
                0,
                0,
                false,
                false,
                true,
                null,
                null)
        {
        }

        public CampaignSiteState(
            string siteId,
            string displayName,
            SiteType siteType,
            bool isDiscovered = false,
            bool isCleared = false,
            int threatLevel = 0,
            bool searchCompleted = false,
            string lootRemainingHint = null,
            float lastVisitedTime = 0f,
            int maxThreatLevel = 5,
            int resourceRichness = 0,
            int lootRemaining = 0,
            bool isExhausted = false,
            bool isOccupied = false,
            bool canBecomeOutpost = true,
            string outpostId = null,
            IReadOnlyList<string> tags = null)
        {
            SiteId = siteId;
            DisplayName = displayName;
            SiteType = siteType;
            IsDiscovered = isDiscovered;
            IsCleared = isCleared;
            ThreatLevel = threatLevel;
            SearchCompleted = searchCompleted;
            LootRemainingHint = lootRemainingHint;
            LastVisitedTime = lastVisitedTime;
            LastUpdatedTime = lastVisitedTime;
            MaxThreatLevel = maxThreatLevel < 0 ? 0 : maxThreatLevel;
            ResourceRichness = resourceRichness < 0 ? 0 : resourceRichness;
            LootRemaining = lootRemaining < 0 ? 0 : lootRemaining;
            IsExhausted = isExhausted;
            IsOccupied = isOccupied;
            CanBecomeOutpost = canBecomeOutpost;
            OutpostId = outpostId;
            VisitCount = 0;

            _tags.Clear();
            if (tags != null)
            {
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

        public string SiteId { get; private set; }
        public string DisplayName { get; private set; }
        public SiteType SiteType { get; private set; }
        public bool IsDiscovered { get; private set; }
        public bool IsCleared { get; private set; }
        public int ThreatLevel { get; private set; }
        public bool SearchCompleted { get; private set; }
        public bool IsSearched
        {
            get { return SearchCompleted; }
        }
        public string LootRemainingHint { get; private set; }
        public float LastVisitedTime { get; private set; }
        public float LastUpdatedTime { get; private set; }
        public int MaxThreatLevel { get; private set; }
        public int ResourceRichness { get; private set; }
        public int LootRemaining { get; private set; }
        public bool IsExhausted { get; private set; }
        public bool IsOccupied { get; private set; }
        public bool CanBecomeOutpost { get; private set; }
        public string OutpostId { get; private set; }
        public int VisitCount { get; private set; }
        public IReadOnlyList<string> Tags
        {
            get { return _tags; }
        }

        public void MarkDiscovered()
        {
            IsDiscovered = true;
        }

        public void MarkCleared()
        {
            IsCleared = true;
            IsOccupied = false;
            ReduceThreat(ThreatLevel);
        }

        public void MarkSearchCompleted()
        {
            MarkSearched();
        }

        public void MarkSearched()
        {
            SearchCompleted = true;
            IsDiscovered = true;
        }

        public void MarkVisited(float lastVisitedTime, int visitCountDelta = 1)
        {
            UpdateLastVisitedTime(lastVisitedTime);
            MarkDiscovered();
            if (visitCountDelta > 0)
            {
                VisitCount += visitCountDelta;
            }
        }

        public void SetThreatLevel(int threatLevel)
        {
            ThreatLevel = ClampThreat(threatLevel);
        }

        public void IncreaseThreat(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            ThreatLevel = ClampThreat(ThreatLevel + amount);
        }

        public void ReduceThreat(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            ThreatLevel = ClampThreat(ThreatLevel - amount);
        }

        public void ReduceLoot(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            LootRemaining -= amount;
            if (LootRemaining < 0)
            {
                LootRemaining = 0;
            }

            ResourceRichness -= amount;
            if (ResourceRichness < 0)
            {
                ResourceRichness = 0;
            }

            if (LootRemaining <= 0)
            {
                MarkExhausted();
            }
        }

        public void SetLootRemaining(int amount)
        {
            LootRemaining = amount < 0 ? 0 : amount;
            if (LootRemaining <= 0)
            {
                MarkExhausted();
            }
        }

        public void SetResourceRichness(int amount)
        {
            ResourceRichness = amount < 0 ? 0 : amount;
        }

        public void SetOccupied(bool isOccupied)
        {
            IsOccupied = isOccupied;
        }

        public void MarkExhausted()
        {
            IsExhausted = true;
            LootRemaining = 0;
            ResourceRichness = 0;
        }

        public void SetOutpost(string outpostId)
        {
            OutpostId = outpostId;
        }

        public bool CanLaunchMission()
        {
            return IsDiscovered && !string.IsNullOrEmpty(SiteId);
        }

        public void UpdateLastVisitedTime(float lastVisitedTime)
        {
            LastVisitedTime = lastVisitedTime < 0f ? 0f : lastVisitedTime;
            LastUpdatedTime = LastVisitedTime;
        }

        public void AddTag(string tag)
        {
            if (string.IsNullOrEmpty(tag) || _tags.Contains(tag))
            {
                return;
            }

            _tags.Add(tag);
        }

        private int ClampThreat(int value)
        {
            if (value < 0)
            {
                return 0;
            }

            if (MaxThreatLevel > 0 && value > MaxThreatLevel)
            {
                return MaxThreatLevel;
            }

            return value;
        }
    }
}
