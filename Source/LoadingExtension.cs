using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace AutoBudget
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private bool slidersEventAlreadySet = false;

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame || mode == LoadMode.NewGameFromScenario)
            {
                //setBudgetSlidersEvent();
            }
        }

        private void setBudgetSlidersEvent()
        {
            if (slidersEventAlreadySet)
            {
                return;
            }
            else
            {
                slidersEventAlreadySet = true;
            }

            AutobudgetObjectsContainer c = Singleton<AutobudgetManager>.instance.container;
            foreach (AutobudgetBase obj in c.AllAutobudgetObjects)
            {
                foreach (bool isNight in new[] { false, true })
                {
                    UISlider slider = Helper.GetBudgetSlider(obj.GetEconomyPanelContainerName(), obj.GetBudgetItemName(), isNight);
                    if (slider != null)
                    {
                        slider.eventValueChanged += delegate (UIComponent component, float value)
                        {
                            if (obj.Enabled)
                            {
                                obj.Enabled = false;
                                Mod.UpdateUI();
                            }
                        };
                    }
                } // End of night/day loop
            } // End of obj loop
        }
    }
}
