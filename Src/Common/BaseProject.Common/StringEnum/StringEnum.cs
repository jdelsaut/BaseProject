using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace BoxApi.Common.StringEnum
{
    public sealed class StringEnum
    {
        private readonly HashSet<string> _all;

        public StringEnum(Type type, params string[] exceptions)
        {
            Debug.Assert(type != null, "Type cannot be null");

            IEnumerable<string> values = from f in type.GetTypeInfo().DeclaredFields
                                         where f.IsPublic
                                         select f.GetValue(null) as string;
            _all = new HashSet<string>(values.Except(exceptions), StringComparer.Ordinal);
        }

        public bool IsValid(string value)
        {
            return _all.Contains(value);
        }

        public bool AllIsValid(ICollection<string> values)
        {
            return _all.IsSupersetOf(values);
        }

        public IReadOnlyCollection<string> RetrieveAll()
        {
            return _all;
        }
    }
}