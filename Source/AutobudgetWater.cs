using System;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace AutoBudget
{
    public class AutobudgetWater : AutobudgetNoVehicles
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetWater d = Singleton<AutobudgetManager>.instance.container.AutobudgetWater;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.AutobudgetBuffer);
                s.WriteInt32(d.BudgetMaxValue);
                s.WriteBool(d.PauseWhenBudgetTooHigh);
                s.WriteInt32(d.TargetWaterStorageRatio);
                s.WriteBool(d.UseHeatingAutobudget);
                s.WriteInt32(d.HeatingBudgetMaxValue);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetWater d = Singleton<AutobudgetManager>.instance.container.AutobudgetWater;
                d.Enabled = s.ReadBool();
                d.AutobudgetBuffer = s.ReadInt32();
                d.BudgetMaxValue = s.ReadInt32();
                d.PauseWhenBudgetTooHigh = s.ReadBool();
                d.TargetWaterStorageRatio = s.ReadInt32();
                //d.UseHeatingAutobudget = s.ReadBool();
                //d.HeatingBudgetMaxValue = s.ReadInt32();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetWater data loaded.");
            }
        }

        private int currentHeatingBudget = 0;
        private int heatingCounter = 0;
        private int heatingRefreshCount = 2;

        public int AutobudgetBuffer = 3; // Percent of capacity
        public int BudgetMaxValue = 140;
        public bool PauseWhenBudgetTooHigh = true;
        public int TargetWaterStorageRatio = 95; // Percent of the water capacity
        public bool UseHeatingAutobudget = true;
        public int HeatingBudgetMaxValue = 110;

        public override string GetEconomyPanelContainerName()
        {
            return "ServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "WaterAndSewage";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.Water;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 4 + 3;
            }
        }

        protected override void setAutobudget()
        {
            DistrictManager dm = Singleton<DistrictManager>.instance;

            // Water
            int waterCapacity = dm.m_districts.m_buffer[0].GetWaterCapacity();
            int waterConsumption = dm.m_districts.m_buffer[0].GetWaterConsumption();
            // Sewage
            int sewageCapacity = dm.m_districts.m_buffer[0].GetSewageCapacity();
            int sewageAccumulation = dm.m_districts.m_buffer[0].GetSewageAccumulation();

            // No water and no sewage
            if (waterCapacity <= 0 && sewageCapacity <= 0) return;

            int waterStorageCapacity = dm.m_districts.m_buffer[0].GetWaterStorageCapacity();
            int waterStorageAmount = dm.m_districts.m_buffer[0].GetWaterStorageAmount();
            int waterStorageRatio = waterStorageCapacity == 0 ? 0 : waterStorageAmount * 100 / waterStorageCapacity;

            AutobudgetObjectsContainer o = Singleton<AutobudgetManager>.instance.container;
            EconomyManager em = Singleton<EconomyManager>.instance;
            SimulationManager sm = Singleton<SimulationManager>.instance;

            int budget = em.GetBudget(ItemClass.Service.Water, ItemClass.SubService.None, sm.m_isNightTime);

            float buffer = getBufferCoefficient(AutobudgetBuffer);
            int newWaterBudget = calculateNewBudget(waterCapacity, waterConsumption, budget, waterStorageRatio < TargetWaterStorageRatio ? buffer : 1f);
            int newSewageBudget = calculateNewBudget(sewageCapacity, sewageAccumulation, budget, buffer);
            int newBudget = Math.Max(newWaterBudget, newSewageBudget);

            newBudget = Math.Min(newBudget, BudgetMaxValue);

            if (newBudget == BudgetMaxValue && PauseWhenBudgetTooHigh && !isPausedRecently)
            {
                SetPause();
                isPausedRecently = true;
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Water, InfoManager.SubInfoMode.Default);
            }

            if (newBudget < BudgetMaxValue)
            {
                isPausedRecently = false;
            }

            // Heating autobudget
            if (UseHeatingAutobudget && newBudget < HeatingBudgetMaxValue)
            {
                if (heatingCounter-- <= 0)
                {
                    heatingCounter = heatingRefreshCount;

                    if (currentHeatingBudget < newBudget)
                    {
                        currentHeatingBudget = newBudget;
                    }

                    bool isHeatingProblem = false;
                    BuildingManager bm = Singleton<BuildingManager>.instance;
                    for (int n = 0; n <= (255 + 1) * 192 - 1; n++)
                    {
                        Building.Flags flags = bm.m_buildings.m_buffer[n].m_flags;
                        if ((flags & Building.Flags.Created) != Building.Flags.None)
                        {
                            if (bm.m_buildings.m_buffer[n].m_heatingProblemTimer > 0)
                            {
                                isHeatingProblem = true;
                                break;
                            }
                        }
                    }

                    if (isHeatingProblem)
                    {
                        currentHeatingBudget += 1;
                    }
                    else
                    {
                        currentHeatingBudget -= 1;
                    }
                }

                if (currentHeatingBudget > newBudget)
                {
                    newBudget = currentHeatingBudget;
                }
            }

            setBudget(newBudget);
        }
    }
}
