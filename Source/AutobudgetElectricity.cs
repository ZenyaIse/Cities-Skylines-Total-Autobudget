using System;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace AutoBudget
{
    public class AutobudgetElectricity : AutobudgetNoVehicles
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetElectricity d = Singleton<AutobudgetManager>.instance.container.AutobudgetElectricity;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.AutobudgetBuffer);
                s.WriteInt32(d.BudgetMaxValue);
                s.WriteBool(d.PauseWhenBudgetTooHigh);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetElectricity d = Singleton<AutobudgetManager>.instance.container.AutobudgetElectricity;
                d.Enabled = s.ReadBool();
                d.AutobudgetBuffer = s.ReadInt32();
                d.BudgetMaxValue = s.ReadInt32();
                d.PauseWhenBudgetTooHigh = s.ReadBool();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                SerializableDataExtension.LastReadDataVersion = s.version;
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetElectricity data loaded.");
            }
        }

        private int capacity_prev = 0;
        private int consumption_prev = 0;

        public int AutobudgetBuffer = 3; // Percent of capacity
        public int BudgetMaxValue = 140;
        public bool PauseWhenBudgetTooHigh = true;

        public AutobudgetElectricity()
        {
            refreshCount = 89;
        }

        public override string GetEconomyPanelContainerName()
        {
            return "ServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "Electricity";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.Electricity;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override void setAutobudget()
        {
            DistrictManager dm = Singleton<DistrictManager>.instance;

            int capacity = dm.m_districts.m_buffer[0].GetElectricityCapacity();

            // No electricity
            if (capacity <= 0) return;

            int consumption = dm.m_districts.m_buffer[0].GetElectricityConsumption();

            // No changes from the previous state
            if (capacity == capacity_prev && consumption == consumption_prev) return;

            capacity_prev = capacity;
            consumption_prev = consumption;

            int electricityFromGarbage = getGarbageElectricityProduction();
            //Debug.Log(string.Format("capacity = {0}, consumption = {1}, getGarbageElectricityProduction = {2}", capacity, consumption, getGarbageElectricityProduction()));

            AutobudgetObjectsContainer o = Singleton<AutobudgetManager>.instance.container;
            EconomyManager em = Singleton<EconomyManager>.instance;
            SimulationManager sm = Singleton<SimulationManager>.instance;

            int budget = em.GetBudget(ItemClass.Service.Electricity, ItemClass.SubService.None, sm.m_isNightTime);
            int newBudget = calculateNewBudget(capacity - electricityFromGarbage, consumption - electricityFromGarbage, budget, getBufferCoefficient(AutobudgetBuffer));

            newBudget = Math.Min(newBudget, BudgetMaxValue);

            if (newBudget == BudgetMaxValue && PauseWhenBudgetTooHigh && !isPausedRecently)
            {
                SetPause();
                isPausedRecently = true;
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Electricity, InfoManager.SubInfoMode.Default);
            }

            if (newBudget < BudgetMaxValue)
            {
                isPausedRecently = false;
            }

            setBudget(newBudget);
        }

        private int getGarbageElectricityProduction()
        {
            int result = 0;

            BuildingManager bm = Singleton<BuildingManager>.instance;

            foreach (ushort n in ServiceBuildingNs(ItemClass.Service.Garbage))
            {
                Building bld = bm.m_buildings.m_buffer[(int)n];
                if ((bld.m_flags & Building.Flags.Active) == 0) continue;

                if (bld.Info.m_buildingAI.GetType() == typeof(LandfillSiteAI))
                {
                    result += ((LandfillSiteAI)bld.Info.m_buildingAI).GetElectricityRate(n, ref bld);
                }
            }

            return result * 16;
        }
    }
}
