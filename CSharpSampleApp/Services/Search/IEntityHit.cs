using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpSampleApp.Services.Search
{
    public interface IEntityHit
    {
        double Rank { get; }

        string Name { get; }
    }
}
