using System;
using ColossalFramework;

namespace AutoBudget
{
    public class AutobudgetTaxi : AutobudgetVehicles
    {
        public override string GetEconomyPanelContainerName()
        {
            return "SubServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "Taxi";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.PublicTransport;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.PublicTransportTaxi;
        }

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 2;
            }
        }

        protected override void setAutobudget()
        {
            int totalCurrentVehicleCount = 0;
            int totalNormalVehicleCount = 0;
            int totalTaxiStandCapacity = 0;
            int vehiclesWaitingAtTaxiStand = 0;
            int taxiDepotCount = 0;

            foreach (ushort n in ServiceBuildingNs(ItemClass.Service.PublicTransport))
            {
                Building bld = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)n];
                if ((bld.m_flags & Building.Flags.Active) == 0) continue;

                if (bld.Info.m_buildingAI is TaxiStandAI)
                {
                    totalTaxiStandCapacity += (bld.Info.m_buildingAI as TaxiStandAI).m_maxVehicleCount;
                    vehiclesWaitingAtTaxiStand += (bld.Info.m_buildingAI as TaxiStandAI).GetVehicleCount(n, ref bld);
                }
                else if (bld.Info.m_buildingAI is DepotAI)
                {
                    taxiDepotCount++;
                    totalCurrentVehicleCount += (bld.Info.m_buildingAI as DepotAI).GetVehicleCount(n, ref bld);
                    totalNormalVehicleCount += (bld.Info.m_buildingAI as DepotAI).m_maxVehicleCount;
                }
            }

            int buffer = Math.Max(taxiDepotCount, totalTaxiStandCapacity * 3 / 5);
            int desiredVehicleCount = totalCurrentVehicleCount - vehiclesWaitingAtTaxiStand + buffer;

            int budget = Singleton<EconomyManager>.instance.GetBudget(ItemClass.Service.PublicTransport,
                    ItemClass.SubService.PublicTransportTaxi, Singleton<SimulationManager>.instance.m_isNightTime);
            int newBudget = budget;

            //if (needToEncreaseCapacityCount > 0)
            //{
            //    // Add 1 vehicle
            //    newBudget = getBudgetNeededToGetOneMoreVehicle(normalVehicleCapacity_Min, budget, maxBudget);
            //}
            //else if (canDecreaseCapacityCount > 0 && perfectVehiclesCount == 0)
            //{
            //    newBudget = getBudgetNeededToGetOneLessVehicle(normalVehicleCapacity_Min, budget, minBudget);
            //}

            if (newBudget != budget)
            {
                Singleton<EconomyManager>.instance.SetBudget(
                    ItemClass.Service.PublicTransport,
                    ItemClass.SubService.PublicTransportTaxi,
                    newBudget,
                    Singleton<SimulationManager>.instance.m_isNightTime
                    );
            }


            //if (!Singleton<BuildingManager>.exists) return;

            //BuildingManager bm = Singleton<BuildingManager>.instance;

            //FastList<ushort> serviceBuildings = bm.GetServiceBuildings(ItemClass.Service.PublicTransport);
            //if (serviceBuildings == null || serviceBuildings.m_buffer == null) return;

            //for (int i = 0; i < serviceBuildings.m_size; i++)
            //{
            //    ushort n = serviceBuildings.m_buffer[i];
            //    if (n == 0) continue;

            //    Building bld = bm.m_buildings.m_buffer[(int)n];
            //    if ((bld.m_flags & Building.Flags.Active) == 0) continue;

            //    if (bld.Info.m_class.m_subService == ItemClass.SubService.PublicTransportTaxi)
            //    {
            //    }
            //}
            //float targetProductionRate = (totalCurrentVehicleCount - vehiclesWaitingAtTaxiStand + buffer) / (float)totalNormalVehicleCount;

            //int newBudget = 1 + getBudgetFromProductionRate(targetProductionRate);
            //newBudget = Math.Min(newBudget, 105);
        }
    }
}
