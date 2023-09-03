using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities
{
    public class AddressEntity
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public long UserId { get; set; }
        public UserEntity User { get; set; }
    }
}
