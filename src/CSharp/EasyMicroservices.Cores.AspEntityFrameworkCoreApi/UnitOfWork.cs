using EasyMicroservices.Cores.AspCoreApi.Interfaces;
using EasyMicroservices.Cores.AspCoreApi.Managers;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Logics;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Models;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore;
using EasyMicroservices.Database.EntityFrameworkCore.Providers;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.CompileTimeMapper.Interfaces;
using EasyMicroservices.Mapper.CompileTimeMapper.Providers;
using EasyMicroservices.Mapper.Interfaces;
using EasyMicroservices.Mapper.SerializerMapper.Providers;
using EasyMicroservices.Serialization.Interfaces;
using EasyMicroservices.Serialization.Newtonsoft.Json.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.AspEntityFrameworkCoreApi
{
    /// <summary>
    /// 
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// 
        /// </summary>
        public static Type MapperTypeAssembly { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IServiceProvider ServiceProvider { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public UnitOfWork(IServiceProvider service)
        {
            service.ThrowIfNull(nameof(service));
            ServiceProvider = service;
        }

        List<object> Disposables { get; set; } = new List<object>();

        T AddDisposable<T>(T data)
        {
            Disposables.Add(data);
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IDatabase GetDatabase()
        {
            if (ServiceProvider == null)
                throw new ObjectDisposedException(nameof(ServiceProvider));
            var context = ServiceProvider.GetService<RelationalCoreContext>();
            if (context == null)
                throw new Exception("RelationalCoreContext is null, please add your context to RelationalCoreContext as Transit or Scope.\r\nExample : services.AddTransient<RelationalCoreContext>(serviceProvider => serviceProvider.GetService<YourDbContext>());");
            return AddDisposable(new EntityFrameworkCoreDatabaseProvider(context));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IContentResolver GetContentResolver()
        {
            return ServiceProvider.GetService<IContentResolver>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IAuthorization GetAuthorization()
        {
            return ServiceProvider.GetService<IAuthorization>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IDatabase GetDatabase<TContext>()
                where TContext : RelationalCoreContext
        {
            if (ServiceProvider == null)
                throw new ObjectDisposedException(nameof(ServiceProvider));
            var context = ServiceProvider.GetService<TContext>();
            if (context == null)
                throw new Exception("TContext is null, please add your context to Context as Transit or Scope.\r\nExample : services.AddTransient<YourContext>(serviceProvider => serviceProvider.GetService<YourDbContext>());");

            return AddDisposable(new EntityFrameworkCoreDatabaseProvider(context));
        }

        #region ContractLogic
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TEntity, TEntity, TEntity, long> GetLongLogic<TEntity>(LogicOptions logicOptions = default)
           where TEntity : class
        {
            return GetInternalLongContractLogic<TEntity, TEntity, TEntity, TEntity>(logicOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IContractLogic<TEntity, TEntity, TEntity, TEntity, TEntity> GetLogic<TEntity>(LogicOptions logicOptions = default) where TEntity : class
        {
            return GetInternalContractLogic<TEntity, TEntity, TEntity, TEntity, TEntity>(logicOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TContract, TContract, TContract, long> GetLongContractLogic<TEntity, TContract>(LogicOptions logicOptions = default)
           where TContract : class
           where TEntity : class
        {
            return GetInternalLongContractLogic<TEntity, TContract, TContract, TContract>(logicOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TEntity, TEntity, TEntity, long> GetLongReadableLogic<TEntity>(LogicOptions logicOptions = default)
           where TEntity : class
        {
            return GetInternalLongContractLogic<TEntity, TEntity, TEntity, TEntity>(logicOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TContract, TContract, TContract, long> GetLongReadableContractLogic<TEntity, TContract>(LogicOptions logicOptions = default)
           where TContract : class
           where TEntity : class
        {
            return GetInternalLongContractLogic<TEntity, TContract, TContract, TContract>(logicOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TEntity, TEntity, TEntity, TId> GetLogic<TEntity, TId>(LogicOptions logicOptions = default)
           where TEntity : class
        {
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TEntity, TEntity, TEntity, TId>(GetDatabase().GetReadableOf<TEntity>(), GetDatabase().GetWritableOf<TEntity>(), this, logicOptions));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TContract, TContract, TContract, TId> GetContractLogic<TEntity, TContract, TId>(LogicOptions logicOptions = default)
           where TContract : class
           where TEntity : class
        {
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TContract, TContract, TContract, TId>(GetDatabase().GetReadableOf<TEntity>(), GetDatabase().GetWritableOf<TEntity>(), this, logicOptions));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TEntity, TEntity, TEntity, TId> GetReadableLogic<TEntity, TId>(LogicOptions logicOptions = default)
           where TEntity : class
        {
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TEntity, TEntity, TEntity, TId>(GetDatabase().GetReadableOf<TEntity>(), this, logicOptions));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TContract, TContract, TContract, TId> GetReadableContractLogic<TEntity, TContract, TId>(LogicOptions logicOptions = default)
           where TContract : class
           where TEntity : class
        {
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TContract, TContract, TContract, TId>(GetDatabase().GetReadableOf<TEntity>(), this, logicOptions));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TCreateRequestContract"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IContractLogic<TEntity, TContract, TCreateRequestContract, TContract, long> GetLongContractLogic<TEntity, TCreateRequestContract, TContract>(LogicOptions logicOptions = default)
            where TContract : class
            where TEntity : class
        {
            return GetInternalLongContractLogic<TEntity, TContract, TCreateRequestContract, TContract>(logicOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResponseContract"></typeparam>
        /// <typeparam name="TCreateRequestContract"></typeparam>
        /// <typeparam name="TUpdateRequestContract"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, long> GetLongContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract>(LogicOptions logicOptions = default)
            where TResponseContract : class
            where TEntity : class
        {
            return GetInternalLongContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract>(logicOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TCreateRequestContract"></typeparam>
        /// <typeparam name="TUpdateRequestContract"></typeparam>
        /// <typeparam name="TResponseContract"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> GetContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>(LogicOptions logicOptions = default)
            where TResponseContract : class
            where TEntity : class
        {
            return GetInternalContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>(logicOptions);
        }

        IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, long> GetInternalLongContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract>(LogicOptions logicOptions = default)
          where TResponseContract : class
          where TEntity : class
        {
            return AddDisposable(new LongIdMappedDatabaseLogicBase<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract>(AddDisposable(GetDatabase().GetReadableOf<TEntity>()), AddDisposable(GetDatabase().GetWritableOf<TEntity>()), this, logicOptions));
        }

        IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> GetInternalContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>(LogicOptions logicOptions = default)
          where TResponseContract : class
          where TEntity : class
        {
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>(AddDisposable(GetDatabase().GetReadableOf<TEntity>()), AddDisposable(GetDatabase().GetWritableOf<TEntity>()), this, logicOptions));
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual IEasyQueryableAsync<TEntity> GetQueryableOf<TEntity>()
             where TEntity : class
        {
            return AddDisposable(GetDatabase().GetQueryOf<TEntity>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual IEasyReadableQueryableAsync<TEntity> GetReadableOf<TEntity>()
             where TEntity : class
        {
            return AddDisposable(GetDatabase().GetReadableOf<TEntity>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual IEasyWritableQueryableAsync<TEntity> GetWritableOf<TEntity>()
             where TEntity : class
        {
            return AddDisposable(GetDatabase().GetWritableOf<TEntity>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IMapperProvider GetMapper()
        {
            var mapper = new CompileTimeMapperProvider(new SerializerMapperProvider(new NewtonsoftJsonProvider(new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                Error = HandleDeserializationError
            })));
            if (MapperTypeAssembly != null)
            {
                foreach (var type in MapperTypeAssembly.Assembly.GetTypes())
                {
                    if (typeof(IMapper).IsAssignableFrom(type))
                    {
                        var instance = Activator.CreateInstance(type, mapper);
                        var returnTypes = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(x => x.ReturnType != typeof(object) && x.Name == "Map").Select(x => x.ReturnType).ToArray();
                        mapper.AddMapper(returnTypes[0], returnTypes[1], (IMapper)instance);
                    }
                }
            }
            return mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="errorArgs"></param>
        public virtual void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            //var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IUniqueIdentityManager UniqueIdentityManager { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IUniqueIdentityManager GetUniqueIdentityManager()
        {
            return ServiceProvider.GetService<IUniqueIdentityManager>();
        }

        static Func<IServiceProvider, Task<WhiteLabelInfo>> _InitializeWhiteLabel;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<WhiteLabelInfo> InitializeWhiteLabel()
        {
            if (ServiceProvider == null)
                throw new ObjectDisposedException(nameof(ServiceProvider));
            _InitializeWhiteLabel.ThrowIfNull(nameof(InitializeWhiteLabel));
            return await _InitializeWhiteLabel(ServiceProvider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="microserviceName"></param>
        /// <param name="whiteLabelRoute"></param>
        /// <param name="dbContextTypes"></param>
        /// <returns></returns>
        public virtual Task InitializeWhiteLabel(string microserviceName, string whiteLabelRoute, params Type[] dbContextTypes)
        {
            if (ServiceProvider == null)
                throw new ObjectDisposedException(nameof(ServiceProvider));
            _InitializeWhiteLabel = async (serviceProvider) =>
            {
                var whiteLabelManager = serviceProvider.GetService<WhiteLabelManager>();
                if (!whiteLabelManager.IsInitialized)
                    return await whiteLabelManager.Initialize(serviceProvider.GetService<IHttpContextAccessor>(), microserviceName, whiteLabelRoute, dbContextTypes);
                return whiteLabelManager.CurrentWhiteLabel;
            };
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="microserviceName"></param>
        /// <param name="dbContextTypes"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public virtual void Initialize(string microserviceName, params Type[] dbContextTypes)
        {
            if (ServiceProvider == null)
                throw new ObjectDisposedException(nameof(ServiceProvider));
            _InitializeWhiteLabel = (serviceProvider) =>
            {
                return Task.FromResult(new WhiteLabelInfo()
                {
                    MicroserviceName = microserviceName
                });
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ITextSerializationProvider GetTextSerialization()
        {
            return ServiceProvider.GetService<ITextSerializationProvider>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<string> GetCurrentUserUniqueIdentity(LogicOptions logicOptions = default)
        {
            await InitializeWhiteLabel();
            var httpContext = ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
            if (httpContext != null)
            {
                var uniqueIdentity = httpContext.User.FindFirst(nameof(IUniqueIdentitySchema.UniqueIdentity))?.Value;
                if (uniqueIdentity.HasValue() && logicOptions.UniqueIdentityStrategy != DataTypes.UniqueIdentityStrategy.Default)
                {
                    if (logicOptions.UniqueIdentityStrategy == DataTypes.UniqueIdentityStrategy.BusinessTwoSegment)
                        return DefaultUniqueIdentityManager.CutUniqueIdentity(uniqueIdentity, 2);
                    else if (logicOptions.UniqueIdentityStrategy == DataTypes.UniqueIdentityStrategy.UserFourSegment)
                        return DefaultUniqueIdentityManager.CutUniqueIdentity(uniqueIdentity, 4);
                    else if (logicOptions.UniqueIdentityStrategy == DataTypes.UniqueIdentityStrategy.ObjectSixSegment)
                        return DefaultUniqueIdentityManager.CutUniqueIdentity(uniqueIdentity, 6);
                }
                return uniqueIdentity;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Dispose()
        {
            InternalSyncDispose();
            _ = InternalDispose();
            Disposables.Clear();
            ServiceProvider = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async ValueTask DisposeAsync()
        {
            InternalSyncDispose();
            await InternalDispose();
            Disposables.Clear();
            ServiceProvider = null;
        }

        async Task InternalDispose()
        {
            foreach (var item in Disposables)
            {
                if (item is IAsyncDisposable disposable)
                {
                    await disposable.DisposeAsync();
                }
            }
        }

        void InternalSyncDispose()
        {
            foreach (var item in Disposables)
            {
                if (item is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IConfiguration GetConfiguration()
        {
            return ServiceProvider.GetService<IConfiguration>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Task<bool> HasUniqueIdentityRole()
        {
            var auth = GetAuthorization();
            if (auth == null)
                return Task.FromResult(true);

            return auth.HasUnlimitedPermission(ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetFullAccessPersonalAccessToken()
        {
            var config = GetConfiguration();
            var section = config?.GetSection("Authorization");
            if (section == null)
                return null;
            return section.GetValue<string>("FullAccessPAT");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<ServiceAddressInfo> ManualServiceAddresses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public virtual List<ServiceAddressInfo> GetServiceAddresses(IConfiguration config)
        {
            if (ManualServiceAddresses != null)
                return ManualServiceAddresses;
            return config.GetSection("ServiceAddresses").Get<List<ServiceAddressInfo>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual ServiceAddressInfo GetServiceAddress(string name)
        {
            return GetServiceAddresses(GetConfiguration())
                ?.Where(x => x.Name.HasValue())
                .FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
