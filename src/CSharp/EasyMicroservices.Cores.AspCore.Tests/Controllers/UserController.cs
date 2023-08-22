using EasyMicroservices.Cores.AspCoreApi;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EasyMicroservices.Cores.AspCore.Tests.Controllers
{
    public class UserController : SimpleQueryServiceController<UserEntity, UserEntity, UserEntity, UserEntity, long>
    {
        public UserController(IContractLogic<UserEntity, UserEntity, UserEntity, UserEntity, long> contractLogic) : base(contractLogic)
        {
        }
    }
}
