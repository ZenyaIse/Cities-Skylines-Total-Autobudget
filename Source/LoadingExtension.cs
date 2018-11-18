using ICities;
using ColossalFramework;

namespace AutoBudget
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame || mode == LoadMode.NewGameFromScenario)
            {
                BudgetControlsManager.Init();
            }
        }

        public override void OnLevelUnloading()
        {
            Singleton<AutobudgetManager>.instance.ReadValuesFromFile();
            Mod.UpdateUI();
            BudgetControlsManager.ResetUI();
        }
    }
}
