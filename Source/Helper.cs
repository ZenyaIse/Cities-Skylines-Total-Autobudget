using System;
using System.Collections.Generic;
using ColossalFramework;

namespace Autobudget
{
    public static class Helper
    {
        public static void SetPause()
        {
            if (Singleton<SimulationManager>.exists)
            {
                Singleton<SimulationManager>.instance.SimulationPaused = true;
            }
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

        public static float GetProductionRateFromBudget(int budget)
        {
            float b = budget / 100.0f;

            if (b < 1f) return b * b;
            if (b >= 1.5f) return 1.25f;
            if (b > 1f) return 3 * b - b * b - 1;

            return b;
        }

        public static int GetBudgetFromProductionRate(float rate)
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
