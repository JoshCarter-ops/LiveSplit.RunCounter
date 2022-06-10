using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public class RunCounterFactory : IComponentFactory
    {
        public string ComponentName => "Run Counter";
        public string Description => "Displays the amount of similar run times you've had close to your pb (minute barriers).";
        public ComponentCategory Category => ComponentCategory.Information;
        public IComponent Create(LiveSplitState state) => new RunCounterComponent(state);
        public string UpdateName => ComponentName;
        public string UpdateURL => "";
        public string XMLURL => UpdateURL + "";
        public Version Version => Version.Parse("1.0.0");
    }
}
