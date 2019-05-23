using System;
using ColossalFramework;

namespace Autobudget
{
    public abstract class AutobudgetNoVehicles : AutobudgetBase
    {
        protected bool isPausedRecently = false;

        protected void SetPause()
        {
            if (Singleton<SimulationManager>.exists)
            {
                Singleton<SimulationManager>.instance.SimulationPaused = true;
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
