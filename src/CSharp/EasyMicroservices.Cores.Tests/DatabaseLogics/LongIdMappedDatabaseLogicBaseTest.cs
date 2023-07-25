using EasyMicroservices.Cores.Contracts.Contracts.Requests;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Logics;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities;
using EasyMicroservices.Database.EntityFrameworkCore.Providers;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.CompileTimeMapper.Providers;
using EasyMicroservices.Mapper.Interfaces;
using EasyMicroservices.Mapper.SerializerMapper.Providers;
using ServiceContracts;

namespace EasyMicroservices.Cores.Tests.Database
{
    public class LongIdMappedDatabaseLogicBaseTest
    {
        public LongIdMappedDatabaseLogicBaseTest()
        {
        }


        IContractLogic<UserEntity, UserEntity, UserEntity, UserEntity, long> GetContractLogic()
        {
            return new LongIdMappedDatabaseLogicBase<UserEntity, UserEntity, UserEntity, UserEntity>(GetDatabase().GetReadableOf<UserEntity>(), GetDatabase().GetWritableOf<UserEntity>(), GetMapper(), GetUniqueIdentityManager());
        }

        public virtual IDatabase GetDatabase()
        {
            return new EntityFrameworkCoreDatabaseProvider(new MyTestContext());
        }

        const long TableContextId = 150;
        public virtual IUniqueIdentityManager GetUniqueIdentityManager()
        {
            var manager = new DefaultUniqueIdentityManager("1-1", 5);
            manager.InitializeTables(5, manager.GetContextName(typeof(MyTestContext)), manager.GetTableName(typeof(UserEntity)), TableContextId);
            return manager;
        }

        public virtual IMapperProvider GetMapper()
        {
            var mapper = new CompileTimeMapperProvider(new SerializerMapperProvider(new EasyMicroservices.Serialization.Newtonsoft.Json.Providers.NewtonsoftJsonProvider()));
            return mapper;
        }

        [Theory]
        [InlineData("Ali")]
        [InlineData("Reza")]
        [InlineData("Javad")]
        public async Task<UserEntity> AddAsync(string userName)
        {
            await using var logic = GetContractLogic();
            var addUser = new UserEntity()
            {
                UserName = userName
            };
            var user = await logic.Add(addUser);
            Assert.True(user.IsSuccess);
            Assert.True(user.Result > 0);
            var foundUser = await logic.GetById(user.Result);
            Assert.True(foundUser.IsSuccess);
            Assert.Equal(user.Result, foundUser.Result.Id);
            Assert.Equal(addUser.UserName, foundUser.Result.UserName);
            Assert.NotEmpty(foundUser.Result.UniqueIdentity);
            var allUsers = await logic.GetAll();
            Assert.Contains(allUsers.Result, x => x.Id == user.Result);
            var ids = DefaultUniqueIdentityManager.DecodeUniqueIdentity(foundUser.Result.UniqueIdentity);
            Assert.Equal(ids.Last(), foundUser.Result.Id);
            Assert.Equal(TableContextId, ids[^2]);
            return foundUser.Result;
        }

        [Theory]
        [InlineData("Mahdi", "Mahdi1")]
        [InlineData("Hassan", "Hassan1")]
        public async Task UpdateAsync(string userName, string toUserName)
        {
            await using var logic = GetContractLogic();
            var added = await AddAsync(userName);
            added.UserName = toUserName;
            await logic.Update(added);
            var found = await logic.GetById(added.Id);
            Assert.Equal(found.Result.UserName, toUserName);
        }

        [Theory]
        [InlineData("Ahmad")]
        [InlineData("Yasin")]
        public async Task UniqueIdentityAsync(string userName)
        {
            await using var logic = GetContractLogic();
            var added = await AddAsync(userName);
            var found = await logic.GetByUniqueIdentity(new GetUniqueIdentityRequest()
            {
                UniqueIdentity = added.UniqueIdentity
            });
            Assert.Equal(found.Result.UserName, userName);

            var foundAll = await logic.GetAllByUniqueIdentity(new GetUniqueIdentityRequest()
            {
                UniqueIdentity = DefaultUniqueIdentityManager.CutUniqueIdentityFromEnd(added.UniqueIdentity, 2)
            });
            Assert.Contains(foundAll.Result, x =>x.UserName == userName);
        }

        [Theory]
        [InlineData("Hossein")]
        public async Task HardDeleteAsync(string userName)
        {
            await using var logic = GetContractLogic();
            var added = await AddAsync(userName);
            var found = await logic.GetById(added.Id);
            Assert.Equal(found.Result.Id, added.Id);
            var deleted = await logic.HardDeleteById(found.Result.Id);
            Assert.True(deleted);
            found = await logic.GetById(added.Id);
            Assert.Equal(FailedReasonType.NotFound, found.Error.FailedReasonType);
        }
    }
}
