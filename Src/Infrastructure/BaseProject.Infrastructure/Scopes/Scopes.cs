using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BoxApi.Infrastructure.Scopes
{
    public static class Scopes
    {
        private static readonly IList<string> _all = All();
        public const string BoxApiRead = "urn:axa.partners.backends.BoxApi.read_only";
        public const string BoxApiWrite = "urn:axa.partners.backends.BoxApi.write";

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
