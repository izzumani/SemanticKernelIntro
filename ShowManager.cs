using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Plugins
{
    public class ShowManager
    {
        [KernelFunction,Description("random theme")]
        public string RandomTheme()
        {
            return new List<string> { "boo", "dishes", "art", "needle", "tank", "police" }[new Random().Next(0, 6)];
        }
    }
}
