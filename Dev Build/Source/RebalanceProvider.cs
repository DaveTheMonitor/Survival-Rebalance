using StudioForge.TotalMiner.API;

namespace DaveTheMonitor
{
    public class RebalancePluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin() => new RebalancePlugin();

        public ITMPluginArcade GetPluginArcade() => null;

        public ITMPluginBlocks GetPluginBlocks() => null;

        public ITMPluginGUI GetPluginGUI() => null;

        public ITMPluginNet GetPluginNet() => null;
    }
}
