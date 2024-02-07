using EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Logics;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Tests.Contracts.Common;
using EasyMicroservices.Cores.Tests.Database;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities;
using EasyMicroservices.Cores.Tests.Fixtures;
using EasyMicroservices.Database.EntityFrameworkCore.Providers;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.CompileTimeMapper.Providers;
using EasyMicroservices.Mapper.Interfaces;
using EasyMicroservices.Mapper.SerializerMapper.Providers;

namespace EasyMicroservices.Cores.Tests.Issues.BugReports._2024._02;
public class BugReport_2024_02_Test : IClassFixture<ServiceProviderFixture>, IClassFixture<WhiteLabelLaboratoryFixture>
{
    private readonly IServiceProvider _serviceProvider;
    public BugReport_2024_02_Test(ServiceProviderFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
    }

    UnitOfWork GetUnitOfWork()
    {
        return new UnitOfWork(_serviceProvider);
    }

    IContractLogic<UserEntity, UserEntity, UserEntity, UserEntity, long> GetContractLogic()
    {
        return new LongIdMappedDatabaseLogicBase<UserEntity, UserEntity, UserEntity, UserEntity>(GetDatabase().GetReadableOf<UserEntity>(), GetDatabase().GetWritableOf<UserEntity>(), GetUnitOfWork());
    }

    IContractLogic<UserEntity, UpdateUserContract, UpdateUserContract, UserEntity, long> GetUpdateContractLogic()
    {
        return new LongIdMappedDatabaseLogicBase<UserEntity, UpdateUserContract, UpdateUserContract, UserEntity>(GetDatabase().GetReadableOf<UserEntity>(), GetDatabase().GetWritableOf<UserEntity>(), GetUnitOfWork());
    }

    public virtual IDatabase GetDatabase()
    {
        return new EntityFrameworkCoreDatabaseProvider(new MyTestContext(new DatabaseBuilder()));
    }

    const long TableContextId = 9;
    const long ProfileTableContextId = 151;
    const long CategoryTableContextId = 152;
    const long SubjectTableContextId = 153;
    public virtual IUniqueIdentityManager GetUniqueIdentityManager()
    {
        var manager = new DefaultUniqueIdentityManager(new Models.WhiteLabelInfo()
        {
            StartUniqueIdentity = "1-1",
            MicroserviceId = 5,
            MicroserviceName = "TestExample"
        });
        manager.InitializeTables(5, manager.GetContextName(typeof(MyTestContext)), manager.GetTableName(typeof(UserEntity)), TableContextId);
        manager.InitializeTables(5, manager.GetContextName(typeof(MyTestContext)), manager.GetTableName(typeof(ProfileEntity)), ProfileTableContextId);
        manager.InitializeTables(5, manager.GetContextName(typeof(MyTestContext)), manager.GetTableName(typeof(CategoryEntity)), CategoryTableContextId);
        manager.InitializeTables(5, manager.GetContextName(typeof(MyTestContext)), manager.GetTableName(typeof(SubjectEntity)), SubjectTableContextId);
        return manager;
    }

    public virtual IMapperProvider GetMapper()
    {
        var mapper = new CompileTimeMapperProvider(new SerializerMapperProvider(new EasyMicroservices.Serialization.Newtonsoft.Json.Providers.NewtonsoftJsonProvider()));
        return mapper;
    }

    /// <summary>
    /// https://github.com/EasyMicroservices/Cores/issues/155
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Bug_FilterPagingDeletedItems_155()
    {
        await using var logic = GetContractLogic();
        List<long> users = new List<long>();
        string prefix = "Bug_155-";
        for (int i = 0; i < 30; i++)
        {
            var addUser = new UserEntity()
            {
                UserName = prefix + Guid.NewGuid().ToString(),
            };
            var userResult = await logic.Add(addUser);
            Assert.True(userResult);
            if (i < 10)
                users.Add(userResult);
        }

        var foundUser = await logic.Filter(new FilterRequestContract
        {
            Index = 0,
            Length = 10
        }, q => q.Where(x => x.UserName.StartsWith("Bug_155-")));

        Assert.True(foundUser.IsSuccess);
        Assert.True(foundUser.Result.Count == 10);

        await using var deleteLogic = GetContractLogic();
        var softDeleteResult = await deleteLogic.SoftDeleteBulkByIds(users);
        Assert.True(softDeleteResult.IsSuccess);

        var foundUser2 = await logic.Filter(new FilterRequestContract
        {
            Index = 0,
            Length = 10
        }, q => q.Where(x => x.UserName.StartsWith("Bug_155-")));

        Assert.True(foundUser2.IsSuccess);
        Assert.True(foundUser2.Result.Count == 10);
        Assert.True(foundUser2.TotalCount == 20);

        var foundUser3 = await logic.Filter(new FilterRequestContract
        {
            Index = 10,
            Length = 10
        }, q => q.Where(x => x.UserName.StartsWith("Bug_155-")));

        Assert.True(foundUser3.IsSuccess);
        Assert.True(foundUser3.Result.Count == 10);
        Assert.True(foundUser3.TotalCount == 20);
        Assert.True(foundUser2.Result.TrueForAll(x => foundUser3.Result.TrueForAll(y => y.Id != x.Id)));
    }
}
