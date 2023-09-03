using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities
{
    public class UserCompanyEntity
    {
        public long UserId { get; set; }
        public long CompanyId { get; set; }
        public UserEntity User { get; set; }
        public CompanyEntity Company { get; set; }
    }
}
