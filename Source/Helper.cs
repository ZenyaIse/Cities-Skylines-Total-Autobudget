using ColossalFramework;
using ColossalFramework.UI;

namespace AutoBudget
{
    public static class Helper
    {
        public static UISlider GetBudgetSlider(string EconomyPanelContainerName, string budgetItemName, bool isNight)
        {
            SimulationManager sm = Singleton<SimulationManager>.instance;

            EconomyPanel ep = ToolsModifierControl.economyPanel;
            //UITabstrip uITabstrip = base.Find<UITabstrip>("EconomyTabstrip");
            //UIButton uIButton3 = uITabstrip.Find<UIButton>("Budget");
            UIComponent container = ep.component.Find(EconomyPanelContainerName); // ServicesBudgetContainer, SubServicesBudgetContainer
            if (container != null)
            {
                UIComponent budgetItem = container.Find(budgetItemName);
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
                    return budgetItem.Find<UISlider>(isNight ? "NightSlider" : "DaySlider");
                }
            }

            return null;
        }
    }
}
