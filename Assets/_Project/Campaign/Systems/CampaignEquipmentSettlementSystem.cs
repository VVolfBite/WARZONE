using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignEquipmentSettlementSystem
    {
        public void MarkWeaponDeployed(CampaignState campaignState, string weaponInstanceId, string missionId = null)
        {
            CampaignWeaponInstanceState weaponInstance;
            if (!TryGetWeaponInstance(campaignState, weaponInstanceId, out weaponInstance))
            {
                return;
            }

            weaponInstance.MarkDeployed(missionId);
        }

        public void ReturnWeapon(CampaignState campaignState, string weaponInstanceId, string missionId = null)
        {
            CampaignWeaponInstanceState weaponInstance;
            if (!TryGetWeaponInstance(campaignState, weaponInstanceId, out weaponInstance))
            {
                return;
            }

            weaponInstance.ReturnToInventory(missionId);
        }

        public void LoseWeapon(CampaignState campaignState, string weaponInstanceId, string missionId = null)
        {
            CampaignWeaponInstanceState weaponInstance;
            if (!TryGetWeaponInstance(campaignState, weaponInstanceId, out weaponInstance))
            {
                return;
            }

            weaponInstance.MarkLost(missionId);
            weaponInstance.SetAvailable(false);
        }

        public void DamageWeapon(CampaignState campaignState, string weaponInstanceId, float durabilityLoss = 0.25f)
        {
            CampaignWeaponInstanceState weaponInstance;
            if (!TryGetWeaponInstance(campaignState, weaponInstanceId, out weaponInstance))
            {
                return;
            }

            weaponInstance.MarkDamaged(durabilityLoss);
        }

        public void ApplyEquipmentSettlement(CampaignState campaignState, IReadOnlyList<CampaignEquipmentSettlement> settlements)
        {
            if (campaignState == null || settlements == null)
            {
                return;
            }

            for (int i = 0; i < settlements.Count; i++)
            {
                CampaignEquipmentSettlement settlement = settlements[i];
                if (settlement == null || string.IsNullOrEmpty(settlement.WeaponInstanceId))
                {
                    continue;
                }

                if (settlement.IsLost)
                {
                    LoseWeapon(campaignState, settlement.WeaponInstanceId, settlement.MissionId);
                    continue;
                }

                if (settlement.IsDamaged)
                {
                    DamageWeapon(campaignState, settlement.WeaponInstanceId, 1f - settlement.Durability);
                }

                if (settlement.IsReturned)
                {
                    ReturnWeapon(campaignState, settlement.WeaponInstanceId, settlement.MissionId);
                }
            }
        }

        private bool TryGetWeaponInstance(CampaignState campaignState, string weaponInstanceId, out CampaignWeaponInstanceState weaponInstance)
        {
            weaponInstance = null;
            if (campaignState == null || campaignState.Inventory == null || string.IsNullOrEmpty(weaponInstanceId))
            {
                return false;
            }

            return campaignState.Inventory.TryGetWeaponInstance(weaponInstanceId, out weaponInstance);
        }
    }
}
