using System.Collections.Generic;

namespace Dotnet.Func.Ext.Collections
{
    public interface IReadOnlySet<elementˈ> : IReadOnlyCollection<elementˈ>
    {
        bool Contains(elementˈ item);
    }
}
