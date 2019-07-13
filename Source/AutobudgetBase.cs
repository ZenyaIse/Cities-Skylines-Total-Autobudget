using System;
using System.Collections.Generic;
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

        public static IEnumerable<ushort> ServiceBuildingNs(ItemClass.Service service)
        {
            if (Singleton<BuildingManager>.exists)
            {
                BuildingManager bm = Singleton<BuildingManager>.instance;

                FastList<ushort> serviceBuildings = bm.GetServiceBuildings(service);
                if (serviceBuildings != null && serviceBuildings.m_buffer != null)
                {
                    for (int i = 0; i < serviceBuildings.m_size; i++)
                    {
                        ushort n = serviceBuildings.m_buffer[i];
                        if (n == 0) continue;

                        yield return n;
                    }
                }
            }
        }

        protected int getBudgetForVehicles(Type AIType, int vehiclesExcessNum, int minBudget, int maxBudget, ItemClass.Service bldService = ItemClass.Service.None)
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
            foreach (ushort n in ServiceBuildingNs(bldService))
            {
                Building bld = bm.m_buildings.m_buffer[(int)n];
                if ((bld.m_flags & Building.Flags.Active) == 0) continue;

                //Debug.Log(bld.Info.m_buildingAI.GetType().ToString());

                if (bld.Info.m_buildingAI.GetType() == AIType)
                {
                    int normalVehicleCapacity = Helper.GetNormalVehicleCapacity(ref bld);
                    int currentVehicleCapacity = (productionRate * normalVehicleCapacity + 99) / 100;
                    int vehiclesInUse = Helper.CountVehiclesInUse(ref bld);

                    if (vehiclesInUse + vehiclesExcessNum == currentVehicleCapacity)
                    {
                        // Perfect number of vehicles
                        newBudget = Math.Max(newBudget, budget);
                    }
                    else
                    {
                        int targetVehiclesCount = vehiclesInUse + vehiclesExcessNum;
                        int bldTargetBudget = Helper.GetMinimumBudgetToGetVehicles(normalVehicleCapacity, targetVehiclesCount, maxBudget);
                        newBudget = Math.Max(newBudget, bldTargetBudget);
                    }

                    targetBldCount++;
                }
            }
            //Debug.Log(string.Format("New budget: {0}", newBudget));

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

            float normalCapacity = capacity / getProductionRate(budget);
            return getBudgetFromProductionRate(bufferCoefficient * consumption / normalCapacity);
        }

        protected float getProductionRate(int budget)
        {
            float b = budget / 100.0f;

            if (b < 1f) return b * b;
            if (b >= 1.5f) return 1.25f;
            if (b > 1f) return 3 * b - b * b - 1;

            return b;
        }

        protected int getBudgetFromProductionRate(float rate)
        {
            if (rate <= 0.25f) return 50;
            if (rate >= 1.25f) return 150;

            float b = 1.0f;

            if (rate < 1)
            {
                b = (float)Math.Sqrt(rate);
            }

            if (rate > 1)
            {
                b = (3 - (float)Math.Sqrt(5 - 4 * rate)) / 2;
            }

            return (int)(b * 100 + 0.49999f);
        }
    }
}
