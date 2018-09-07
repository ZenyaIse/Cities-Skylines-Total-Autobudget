using ICities;
using ColossalFramework;

namespace AutoBudget
{
    public class Threading: ThreadingExtensionBase
    {
        public override void OnAfterSimulationFrame()
        {
            Singleton<AutobudgetManager>.instance.SetAutobudgetAll();
        }
    }
}
