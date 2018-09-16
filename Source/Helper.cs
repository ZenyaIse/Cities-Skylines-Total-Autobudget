using System.Text;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

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

            //Roads(ColossalFramework.UI.UIPanel)
            //Bus(ColossalFramework.UI.UIPanel)
            //Tram(ColossalFramework.UI.UIPanel)
            //Metro(ColossalFramework.UI.UIPanel)
            //Train(ColossalFramework.UI.UIPanel)
            //Ship(ColossalFramework.UI.UIPanel)
            //Plane(ColossalFramework.UI.UIPanel)
            //Monorail(ColossalFramework.UI.UIPanel)
            //CableCar(ColossalFramework.UI.UIPanel)
            //Taxi(ColossalFramework.UI.UIPanel)
            //Tours(ColossalFramework.UI.UIPanel)

            if (container != null)
            {
                //if (EconomyPanelContainerName == "SubServicesBudgetContainer")
                //{
                //    StringBuilder sb = new StringBuilder("NoTaxMultiplierMod: ");
                //    foreach (UIComponent c in container.components)
                //    {
                //        sb.AppendLine(c.ToString());
                //    }
                //    Debug.Log(sb.ToString());
                //}

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
