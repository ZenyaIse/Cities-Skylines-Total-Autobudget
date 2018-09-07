using System;
using ColossalFramework;
using ColossalFramework.UI;

namespace AutoBudget
{
    public abstract class AutobudgetBase
    {
        protected const int oneDayFrames = 585;
        protected int counter = 0;

        public bool Enabled = true;

        public void SetAutobudget()
        {
            if (!Enabled) return;

            if (!Singleton<DistrictManager>.exists || !Singleton<EconomyManager>.exists || !Singleton<SimulationManager>.exists) return;

            if (counter-- > 0) return;
            counter = refreshCount;

            setAutobudget();
        }

        protected abstract int refreshCount { get; }

        protected abstract void setAutobudget();


        private string getBudgetContainerName(ItemClass.Service service)
        {
            switch (service)
            {
                case ItemClass.Service.Electricity:
                    return "Electricity";
                case ItemClass.Service.Water:
                    return "WaterAndSewage";
                case ItemClass.Service.Garbage:
                    return "Garbage";
                case ItemClass.Service.HealthCare:
                    return "Healthcare";
                case ItemClass.Service.PoliceDepartment:
                    return "Police";
                case ItemClass.Service.Education:
                    return "Education";
                case ItemClass.Service.FireDepartment:
                    return "FireDepartment";
                case ItemClass.Service.Road:
                    break;
            }

            return "None";
        }

        protected void setBudget(ItemClass.Service service, ItemClass.SubService subservice, int newBudget)
        {
            SimulationManager sm = Singleton<SimulationManager>.instance;

            EconomyPanel ep = ToolsModifierControl.economyPanel;
            //UITabstrip uITabstrip = base.Find<UITabstrip>("EconomyTabstrip");
            //UIButton uIButton3 = uITabstrip.Find<UIButton>("Budget");
            UIComponent container = ep.component.Find("ServicesBudgetContainer"); // SubServicesBudgetContainer
            if (container != null)
            {
                UIComponent budgetItem = container.Find(getBudgetContainerName(service));
                if (budgetItem != null)
                {
                    // List of controls:
                    // SliderBackground (ColossalFramework.UI.UISlicedSprite)
                    // Icon (ColossalFramework.UI.UISprite)
                    // DaySlider (ColossalFramework.UI.UISlider)
                    // NightPercentage (ColossalFramework.UI.UILabel)
                    // DayPercentage (ColossalFramework.UI.UILabel)
                    // Total (ColossalFramework.UI.UILabel)
                    // NightSlider (ColossalFramework.UI.UISlider)
                    UISlider slider = budgetItem.Find<UISlider>(sm.m_isNightTime ? "NightSlider" : "DaySlider");
                    if (slider.value != newBudget)
                    {
                        slider.value = newBudget;
                    }
                    return;
                }
            }

            // If the above did not worked, set the budget directly
            Singleton<EconomyManager>.instance.SetBudget(service, subservice, newBudget, sm.m_isNightTime);
        }
    }
}
