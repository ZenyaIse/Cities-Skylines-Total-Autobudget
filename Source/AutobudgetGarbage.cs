using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace Autobudget
{
    public class AutobudgetGarbage : AutobudgetBase
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetGarbage d = Singleton<AutobudgetManager>.instance.container.AutobudgetGarbage;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.BudgetMaxValue);
                s.WriteInt32(d.MaximumGarbageAmount);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetGarbage d = Singleton<AutobudgetManager>.instance.container.AutobudgetGarbage;
                d.Enabled = s.ReadBool();
                d.BudgetMaxValue = s.ReadInt32();
                d.MaximumGarbageAmount = s.ReadInt32();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetGarbage data loaded.");
            }
        }

        public int BudgetMaxValue = 120;
        public int MaximumGarbageAmount = 80; // Percents of capacity (for inceneration plant and recycling center)

        public AutobudgetGarbage()
        {
            refreshCount = 307;
        }

        public override string GetEconomyPanelContainerName()
        {
            return "ServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "Garbage";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.Garbage;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override void setAutobudget()
        {
            BuildingManager bm = Singleton<BuildingManager>.instance;
            foreach (ushort n in Helper.ServiceBuildingNs(ItemClass.Service.Garbage))
            {
                Building bld = bm.m_buildings.m_buffer[(int)n];
                if ((bld.m_flags & Building.Flags.Active) == 0) continue;

                if (bld.Info.m_buildingAI.GetType() == typeof(LandfillSiteAI))
                {
                    LandfillSiteAI ai = (LandfillSiteAI)bld.Info.m_buildingAI;
                    if (ai.m_electricityProduction > 0 || ai.m_materialProduction > 0) // If inceneration plant or recycling center
                    {
                        if (ai.m_garbageCapacity > 0 && ai.GetGarbageAmount(n, ref bld) * 100 / ai.m_garbageCapacity > MaximumGarbageAmount)
                        {
                            setBudget(BudgetMaxValue);
                            return;
                        }
                    }
                }
            }

            setBudget(getBudgetForVehicles(typeof(LandfillSiteAI), 1, 50, BudgetMaxValue));
        }
    }
}
