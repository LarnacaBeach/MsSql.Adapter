using System.Linq;

namespace MsSql.Adapter.Collector.Types
{
    public static class Helpers
    {
        public static bool IsOperationResult(this ResponseItem item)
        {
            if (item.Params.Count != 2) return false;
            if (!item.Params.Any(x => x.Name == "StatusCode")) return false;
            if (!item.Params.Any(x => x.Name == "StatusMessage")) return false;
            return true;
        }
    }
}