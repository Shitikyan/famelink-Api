using System.Reflection;

namespace FameLink.Domain.Helpers
{
    public static class DomainAssemblyIndicator
    {
        public static Assembly GetAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
    }
}
