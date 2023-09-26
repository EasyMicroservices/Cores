using EasyMicroservices.Cores.Database.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities
{
    public class SubjectEntity : FullAbilityIdSchema<long>
    {
        public string Name { get; set; }
    }
}

