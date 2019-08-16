using System;
using ColossalFramework;
using ColossalFramework.UI;

namespace Autobudget
{
    public abstract class AutobudgetBase
    {
        protected const int oneDayFrames = 585;
        protected int refreshCount = oneDayFrames / 2;
        protected int counter = 0;

        private int prevBudgetDay = 0;
        private int prevBudgetNight = 0;

        protected bool isPausedRecently = false;

        public bool Enabled = true;

        public void SetAutobudget()
        {
            if (!Enabled) return;

            if (!Singleton<DistrictManager>.exists || !Singleton<EconomyManager>.exists || !Singleton<SimulationManager>.exists) return;

            EconomyManager em = Singleton<EconomyManager>.instance;
            int budgetDay = em.GetBudget(GetService(), GetSubService(), false);
            int budgetNight = em.GetBudget(GetService(), GetSubService(), true);

            // If not the beginning
            if (prevBudgetDay != 0 && prevBudgetNight != 0)
            {
                // Probably somebody changed budget manually -> disable autobudget
                if (prevBudgetDay != budgetDay || budgetNight != prevBudgetNight)
                {
                    Enabled = false;
                    prevBudgetDay = 0;
                    prevBudgetNight = 0;
                    Mod.UpdateUI();
                    BudgetControlsManager.UpdateControls();
                }
            }

            prevBudgetDay = budgetDay;
            prevBudgetNight = budgetNight;

            if (counter-- <= 0)
            {
                counter = refreshCount;
                setAutobudget();
                prevBudgetDay = 0;
                prevBudgetNight = 0;
            }
        }

        public void SetAutobudgetNow()
        {
            counter = 0;
            SetAutobudget();
        }

        public abstract string GetEconomyPanelContainerName();
        public abstract string GetBudgetItemName();

        public abstract ItemClass.Service GetService();
        public abstract ItemClass.SubService GetSubService();

        protected abstract void setAutobudget();
        
        protected void setBudget(int newBudget)
        {
            if (newBudget == -1) return;

            SimulationManager sm = Singleton<SimulationManager>.instance;

            // Set the budget sliders
            if (BudgetControlsManager.IsBudgetPanelVisible())
            {
                UISlider slider = BudgetControlsManager.GetBudgetSlider(GetEconomyPanelContainerName(), GetBudgetItemName(), sm.m_isNightTime);
                if (slider != null)
                {
                    if (slider.value != newBudget)
                    {
                        slider.value = newBudget;
                    }
                }
            }

            // Set the budget directly
            Singleton<EconomyManager>.instance.SetBudget(GetService(), GetSubService(), newBudget, sm.m_isNightTime);
        }

        protected int getBudgetForVehicles(Type AIType, int vehiclesExcessNum, int minBudget, int maxBudget,
            bool isSecondary = false, ItemClass.Service bldService = ItemClass.Service.None)
        {
            if (!Singleton<BuildingManager>.exists) return 100;

            int budget = Singleton<EconomyManager>.instance.GetBudget(GetService(), GetSubService(), Singleton<SimulationManager>.instance.m_isNightTime);
            int productionRate = PlayerBuildingAI.GetProductionRate(100, budget);

            int newBudget = minBudget;
            int targetBldCount = 0;

            if (bldService == ItemClass.Service.None)
            {
                bldService = GetService();
            }

            BuildingManager bm = Singleton<BuildingManager>.instance;
            foreach (ushort n in Helper.ServiceBuildingNs(bldService))
            {
                Building bld = bm.m_buildings.m_buffer[(int)n];
                if ((bld.m_flags & Building.Flags.Active) == 0) continue;

                if (bld.Info.m_buildingAI.GetType() == AIType)
                {
                    int normalVehicleCapacity = VehiclesHelper.GetNormalVehicleCapacity(ref bld, isSecondary);
                    int currentVehicleCapacity = (productionRate * normalVehicleCapacity + 99) / 100;
                    int vehiclesInUse = VehiclesHelper.CountVehiclesInUse(ref bld, isSecondary);

                    if (vehiclesInUse + vehiclesExcessNum == currentVehicleCapacity)
                    {
                        // Perfect number of vehicles
                        newBudget = Math.Max(newBudget, budget);
                    }
                    else
                    {
                        int targetVehiclesCount = vehiclesInUse + vehiclesExcessNum;
                        int bldTargetBudget = VehiclesHelper.GetMinimumBudgetToGetVehicles(normalVehicleCapacity, targetVehiclesCount, maxBudget);
                        newBudget = Math.Max(newBudget, bldTargetBudget);
                    }

                    targetBldCount++;
                }
            }

            if (targetBldCount > 0)
            {
                return newBudget;
            }
            else
            {
                return -1;
            }
        }

        protected float getBufferCoefficient(int bufferPercent)
        {
            return 1 + 0.01f * bufferPercent;
        }

        protected int calculateNewBudget(int capacity, int consumption, int budget, float bufferCoefficient)
        {
            if (capacity <= 0) return 50;

            float normalCapacity = capacity / Helper.GetProductionRateFromBudget(budget);
            return Helper.GetBudgetFromProductionRate(bufferCoefficient * consumption / normalCapacity);
        }
    }
}
