using System;
using System.Collections.Generic;
using System.Text;

namespace BaseProject.Infrastructure.SmartID
{
    public interface ISmartIDGenerator
    {
        string Generate(Guid id);
        string Generate(string id);
    }
}
