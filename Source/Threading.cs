using ICities;
using ColossalFramework;

namespace Autobudget
{
    public class Threading: ThreadingExtensionBase
    {
        public override void OnAfterSimulationFrame()
        {
            Singleton<AutobudgetManager>.instance.SetAutobudgetAll();
        }
    }
}
