using ICities;
using ColossalFramework;

namespace GameSpeedMod
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            EconomyManager em = Singleton<EconomyManager>.instance;
            GameSpeedOptionsManager gs = Singleton<GameSpeedOptionsManager>.instance;

            if (mode == LoadMode.NewGame)
            {
                int moneyToAdd = 50 * (gs.Options.ConstructionCostMultiplier - 1) * 100000;
                em.AddResource(EconomyManager.Resource.LoanAmount, moneyToAdd, ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Level.None);
            }

            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
            {
                Loans.SetLoans();
            }
        }

        public override void OnLevelUnloading()
        {
            Loans.ResetLoans();
        }
    }
}
