using EasyMicroservices.Cores.AspCoreApi;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Tests.Contracts.Common;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities;

namespace EasyMicroservices.Cores.AspCore.Tests.Controllers;

public class UserMultilingualSimpleController : MultilingualSimpleQueryServiceController<UserEntity, MultiLanguageUserContract, UserContract, UserContract, MultiLanguageUserContract, long>
{
    public UserMultilingualSimpleController(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}