using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BaseProject.Infrastructure.Scopes
{
    public static class Scopes
    {
        private static readonly IList<string> _all = All();
        public const string BaseProjectRead = "urn:axa.partners.backends.BaseProject.read_only";
        public const string BaseProjectWrite = "urn:axa.partners.backends.BaseProject.write";

        private static IList<string> All()
        {
            IEnumerable<string> scopes = from f in typeof(Scopes).GetFields()
                                         where f.IsPublic
                                         select f.GetValue(null) as string;

            return new ReadOnlyCollection<string>(scopes.ToList());
        }

        public static IEnumerable<string> RetrieveAll()
        {
            return _all;
        }
    }
}
