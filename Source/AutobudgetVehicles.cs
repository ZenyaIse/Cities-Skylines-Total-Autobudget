using System;
using System.Collections.Generic;
using ColossalFramework;
using UnityEngine;

namespace AutoBudget
{
    public abstract class AutobudgetVehicles : AutobudgetBase
    {
        protected int countVehiclesInUse(ref Building bld)
        {
            if (bld.Info.m_buildingAI is HospitalAI)
            {
                return countVehiclesInUse(ref bld, TransferManager.TransferReason.Sick);
            }
            if (bld.Info.m_buildingAI is LandfillSiteAI)
            {
                return countVehiclesInUse(ref bld, TransferManager.TransferReason.Garbage);
            }
            if (bld.Info.m_buildingAI is FireStationAI)
            {
                return countVehiclesInUse(ref bld, TransferManager.TransferReason.Fire);
            }
            if (bld.Info.m_buildingAI is PoliceStationAI)
            {
                return countVehiclesInUse(ref bld, TransferManager.TransferReason.Crime);
            }
            if (bld.Info.m_buildingAI is SnowDumpAI)
            {
                return countVehiclesInUse(ref bld, TransferManager.TransferReason.Snow);
            }

            return 0;
        }

        protected int countVehiclesInUse(ref Building bld, TransferManager.TransferReason reason)
        {
            int vehiclesCount = 0;

            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort n = bld.m_ownVehicles;
            int counter = 0;
            while (n != 0)
            {
                if ((TransferManager.TransferReason)instance.m_vehicles.m_buffer[(int)n].m_transferType == reason)
                {
                    vehiclesCount++;
                }
                n = instance.m_vehicles.m_buffer[(int)n].m_nextOwnVehicle;
                if (++counter > 16384)
                {
                    //CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }

            return vehiclesCount;
        }

        private int getNormalVehicleCapacity(ref Building bld)
        {
            if (bld.Info.m_buildingAI is HospitalAI)
            {
                return (bld.Info.m_buildingAI as HospitalAI).m_ambulanceCount;
            }
            if (bld.Info.m_buildingAI is LandfillSiteAI)
            {
                return (bld.Info.m_buildingAI as LandfillSiteAI).m_garbageTruckCount;
            }
            if (bld.Info.m_buildingAI is FireStationAI)
            {
                return (bld.Info.m_buildingAI as FireStationAI).m_fireTruckCount;
            }
            if (bld.Info.m_buildingAI is PoliceStationAI)
            {
                return (bld.Info.m_buildingAI as PoliceStationAI).m_policeCarCount;
            }
            if (bld.Info.m_buildingAI is SnowDumpAI)
            {
                return (bld.Info.m_buildingAI as SnowDumpAI).m_snowTruckCount;
            }

            throw new Exception("getNormalVehicleCapacity from " + bld.Info.name + " is not implemented.");
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

        public static IEnumerable<Building> ServiceBuildings(ItemClass.Service service)
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

                        Building bld = bm.m_buildings.m_buffer[(int)n];
                        if ((bld.m_flags & Building.Flags.Active) == 0) continue;

                        yield return bld;
                    }
                }
            }
        }

        protected int getMinimumBudgetToGetVehicles(int normalVehicleCapacity, int requiredVehiclesCount, int currentBudget)
        {
            int productionRate;
            do
            {
                requiredVehiclesCount--;
                productionRate = requiredVehiclesCount * 100 / normalVehicleCapacity;
                if (productionRate < 25) return 50;
            } while (productionRate >= 125);

            int budget;
            if (productionRate < 100)
            {
                budget = (int)Math.Sqrt(productionRate * 100);
            }
            else if (productionRate > 100)
            {
                budget = (int)(150 - 50 * Math.Sqrt(9 - 4 * (1 + productionRate / 100)));
            }
            else
            {
                budget = 100;
            }
            budget += 1;

            return budget;
        }

        protected void setBudgetForVehicles(Type AIType, int vehiclesExcessNum, int minBudget, int maxBudget)
        {
            if (!Singleton<BuildingManager>.exists) return;

            int budget = Singleton<EconomyManager>.instance.GetBudget(GetService(), GetSubService(), Singleton<SimulationManager>.instance.m_isNightTime);
            int productionRate = PlayerBuildingAI.GetProductionRate(100, budget);

            int newBudget = minBudget;
            int targetBldCount = 0;

            BuildingManager bm = Singleton<BuildingManager>.instance;
            foreach (ushort n in ServiceBuildingNs(GetService()))
            {
                Building bld = bm.m_buildings.m_buffer[(int)n];
                if ((bld.m_flags & Building.Flags.Active) == 0) continue;

                if (bld.Info.m_buildingAI.GetType() == AIType)
                {
                    int normalVehicleCapacity = getNormalVehicleCapacity(ref bld);
                    int currentVehicleCapacity = (productionRate * normalVehicleCapacity + 99) / 100;
                    int vehiclesInUse = countVehiclesInUse(ref bld);

                    if (vehiclesInUse + vehiclesExcessNum == currentVehicleCapacity)
                    {
                        // Perfect number of vehicles
                        newBudget = Math.Max(newBudget, budget);
                    }
                    else
                    {
                        int targetVehiclesCount = vehiclesInUse + vehiclesExcessNum;
                        int bldTargetBudget = getMinimumBudgetToGetVehicles(normalVehicleCapacity, targetVehiclesCount, budget);
                        newBudget = Math.Max(newBudget, bldTargetBudget);
                    }

                    targetBldCount++;
                }
            }
            //Debug.Log(string.Format("New budget: {0}", newBudget));

            if (targetBldCount > 0 && newBudget != budget)
            {
                setBudget(newBudget);
            }
        }
    }
}
