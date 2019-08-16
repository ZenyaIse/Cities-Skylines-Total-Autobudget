using ColossalFramework;
using ColossalFramework.IO;
using System.Collections;
using UnityEngine;

namespace Autobudget
{
    public class AutobudgetHeating : AutobudgetBase
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetHeating d = Singleton<AutobudgetManager>.instance.container.AutobudgetHeating;
                s.WriteBool(d.Enabled);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetHeating d = Singleton<AutobudgetManager>.instance.container.AutobudgetHeating;
                d.Enabled = s.ReadBool();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetHeating data loaded.");
            }
        }

        private int heatingCapacity_prev = 0;
        private int heatingConsumption_prev = 0;

        public AutobudgetHeating()
        {
            refreshCount = 599;
        }

        public override string GetEconomyPanelContainerName()
        {
            return null;
        }

        public override string GetBudgetItemName()
        {
            return null;
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.None;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override void setAutobudget()
        {
            DistrictManager dm = Singleton<DistrictManager>.instance;

            int heatingCapacity = dm.m_districts.m_buffer[0].GetHeatingCapacity();
            int heatingConsumption = dm.m_districts.m_buffer[0].GetHeatingConsumption();

            if (heatingCapacity <= 0 && heatingConsumption <= 0)
            {
                return;
            }

            // Check changes from the previous time
            if (heatingCapacity == heatingCapacity_prev && heatingConsumption == heatingConsumption_prev)
            {
                return;
            }

            heatingCapacity_prev = heatingCapacity;
            heatingConsumption_prev = heatingConsumption;

            int heatingProductionExcess = heatingCapacity - heatingConsumption;

            BuildingManager bm = Singleton<BuildingManager>.instance;
            foreach (ushort n in Helper.ServiceBuildingNs(ItemClass.Service.Water))
            {
                HeatingPlantAI ai = bm.m_buildings.m_buffer[(int)n].Info.m_buildingAI as HeatingPlantAI;
                if (ai != null)
                {
                    int heatingRate = ai.GetHeatingRate(n, ref bm.m_buildings.m_buffer[(int)n]) * 16;

                    if (heatingConsumption > 0 && heatingProductionExcess < 0)
                    {
                        if (bm.m_buildings.m_buffer[(int)n].m_productionRate == 0)
                        {
                            ai.SetProductionRate(n, ref bm.m_buildings.m_buffer[(int)n], 100);
                            return;
                        }
                    }
                    else if (heatingConsumption == 0 || heatingRate <= heatingProductionExcess)
                    {
                        if ((bm.m_buildings.m_buffer[(int)n].m_flags & Building.Flags.Active) == Building.Flags.Active)
                        {
                            ai.SetProductionRate(n, ref bm.m_buildings.m_buffer[(int)n], 0);
                            heatingProductionExcess -= heatingRate;
                        }
                    }
                }
            }
        }
    }
}
