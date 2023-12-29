using EasyMicroservices.Cores.AspCoreApi.Interfaces;
using EasyMicroservices.Cores.AspCoreApi.Managers;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Logics;
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
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        protected IServiceProvider ServiceProvider { get; set; }
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TEntity, TEntity, TEntity, long> GetLongLogic<TEntity>()
           where TEntity : class
        {
            return GetInternalLongContractLogic<TEntity, TEntity, TEntity, TEntity>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IContractLogic<TEntity, TEntity, TEntity, TEntity, TEntity> GetLogic<TEntity>() where TEntity : class
        {
            return GetInternalContractLogic<TEntity, TEntity, TEntity, TEntity, TEntity>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TContract, TContract, TContract, long> GetLongContractLogic<TEntity, TContract>()
           where TContract : class
           where TEntity : class
        {
            return GetInternalLongContractLogic<TEntity, TContract, TContract, TContract>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TEntity, TEntity, TEntity, long> GetLongReadableLogic<TEntity>()
           where TEntity : class
        {
            return GetInternalLongContractLogic<TEntity, TEntity, TEntity, TEntity>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TContract, TContract, TContract, long> GetLongReadableContractLogic<TEntity, TContract>()
           where TContract : class
           where TEntity : class
        {
            return GetInternalLongContractLogic<TEntity, TContract, TContract, TContract>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TEntity, TEntity, TEntity, TId> GetLogic<TEntity, TId>()
           where TEntity : class
        {
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TEntity, TEntity, TEntity, TId>(GetDatabase().GetReadableOf<TEntity>(), GetDatabase().GetWritableOf<TEntity>(), this));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TContract, TContract, TContract, TId> GetContractLogic<TEntity, TContract, TId>()
           where TContract : class
           where TEntity : class
        {
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TContract, TContract, TContract, TId>(GetDatabase().GetReadableOf<TEntity>(), GetDatabase().GetWritableOf<TEntity>(), this));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TEntity, TEntity, TEntity, TId> GetReadableLogic<TEntity, TId>()
           where TEntity : class
        {
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TEntity, TEntity, TEntity, TId>(GetDatabase().GetReadableOf<TEntity>(), this));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TContract, TContract, TContract, TId> GetReadableContractLogic<TEntity, TContract, TId>()
           where TContract : class
           where TEntity : class
        {
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TContract, TContract, TContract, TId>(GetDatabase().GetReadableOf<TEntity>(), this));
        }

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
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TCreateRequestContract"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IContractLogic<TEntity, TContract, TCreateRequestContract, TContract, long> GetLongContractLogic<TEntity, TCreateRequestContract, TContract>()
            where TContract : class
            where TEntity : class
        {
            return GetInternalLongContractLogic<TEntity, TContract, TCreateRequestContract, TContract>();
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
        public virtual IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, long> GetLongContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract>()
            where TResponseContract : class
            where TEntity : class
        {
            return GetInternalLongContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract>();
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
        public virtual IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> GetContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>()
            where TResponseContract : class
            where TEntity : class
        {
            return GetInternalContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>();
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
        /// <param name="whiteLableRoute"></param>
        /// <param name="dbContextTypes"></param>
        /// <returns></returns>
        public virtual Task InitializeWhiteLabel(string microserviceName, string whiteLableRoute, params Type[] dbContextTypes)
        {
            if (ServiceProvider == null)
                throw new ObjectDisposedException(nameof(ServiceProvider));
            _InitializeWhiteLabel = async (serviceProvider) =>
            {
                var whiteLabelManager = serviceProvider.GetService<WhiteLabelManager>();
                if (!whiteLabelManager.IsInitialized)
                   return await whiteLabelManager.Initialize(serviceProvider.GetService<IHttpContextAccessor>(), microserviceName, whiteLableRoute, dbContextTypes);
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

        IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, long> GetInternalLongContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract>()
          where TResponseContract : class
          where TEntity : class
        {
            return AddDisposable(new LongIdMappedDatabaseLogicBase<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract>(AddDisposable(GetDatabase().GetReadableOf<TEntity>()), AddDisposable(GetDatabase().GetWritableOf<TEntity>()), this));
        }

        IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> GetInternalContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>()
          where TResponseContract : class
          where TEntity : class
        {
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>(AddDisposable(GetDatabase().GetReadableOf<TEntity>()), AddDisposable(GetDatabase().GetWritableOf<TEntity>()), this));
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
        public async Task<string> GetCurrentUserUniqueIdentity()
        {
            await InitializeWhiteLabel();
            var httpContext =  ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
            if (httpContext != null)
                return httpContext.User.FindFirst(nameof(IUniqueIdentitySchema.UniqueIdentity))?.Value;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Dispose()
        {
            InternalSyncDispose();
            _ = InternalDispsose();
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
            await InternalDispsose();
            Disposables.Clear();
            ServiceProvider = null;
        }

        async Task InternalDispsose()
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
        public virtual bool HasUniqueIdentityRole()
        {
            var auth = GetAuthorization();
            return auth == null;
        }
    }
}
