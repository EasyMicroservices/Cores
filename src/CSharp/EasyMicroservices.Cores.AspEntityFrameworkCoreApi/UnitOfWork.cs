using EasyMicroservices.Cores.AspCoreApi.Interfaces;
using EasyMicroservices.Cores.AspCoreApi.Managers;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Logics;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore;
using EasyMicroservices.Database.EntityFrameworkCore.Providers;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.CompileTimeMapper.Interfaces;
using EasyMicroservices.Mapper.CompileTimeMapper.Providers;
using EasyMicroservices.Mapper.Interfaces;
using EasyMicroservices.Mapper.SerializerMapper.Providers;
using EasyMicroservices.Serialization.Newtonsoft.Json.Providers;
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
        protected IServiceProvider _service;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public UnitOfWork(IServiceProvider service)
        {
            service.ThrowIfNull(nameof(service));
            _service = service;
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
            if (_service == null)
                throw new ObjectDisposedException(nameof(_service));
            var context = _service.GetService<RelationalCoreContext>();
            if (context == null)
                throw new Exception("RelationalCoreContext is null, please add your context to RelationalCoreContext as Transit or Scope.\r\nExample : services.AddTransient<RelationalCoreContext>(serviceProvider => serviceProvider.GetService<YourDbContext>());");
            return AddDisposable(new EntityFrameworkCoreDatabaseProvider(context));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IAuthorization GetAuthorization()
        {
            return _service.GetService<IAuthorization>();
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
            if (_service == null)
                throw new ObjectDisposedException(nameof(_service));
            var context = _service.GetService<TContext>();
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
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TEntity, TEntity, TEntity, TId>(GetDatabase().GetReadableOf<TEntity>(), GetDatabase().GetWritableOf<TEntity>(), GetMapper(), GetUniqueIdentityManager()));
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
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TContract, TContract, TContract, TId>(GetDatabase().GetReadableOf<TEntity>(), GetDatabase().GetWritableOf<TEntity>(), GetMapper(), GetUniqueIdentityManager()));
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
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TEntity, TEntity, TEntity, TId>(GetDatabase().GetReadableOf<TEntity>(), GetMapper(), GetUniqueIdentityManager()));
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
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TContract, TContract, TContract, TId>(GetDatabase().GetReadableOf<TEntity>(), GetMapper(), GetUniqueIdentityManager()));
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
        /// <returns></returns>
        public virtual IEasyReadableQueryableAsync<TEntity> GetReadableQueryable<TEntity>()
            where TEntity : class
        {
            return AddDisposable(GetDatabase().GetReadableOf<TEntity>());
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
        public static string DefaultUniqueIdentity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static long MicroserviceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static string MicroserviceName { get; set; }
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
            if (UniqueIdentityManager == null)
            {
                if (DefaultUniqueIdentity.HasValue())
                    UniqueIdentityManager = new DefaultUniqueIdentityManager(DefaultUniqueIdentity, MicroserviceId, MicroserviceName);
                else
                    UniqueIdentityManager = new DefaultUniqueIdentityManager(MicroserviceId, MicroserviceName);
            }
            return UniqueIdentityManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="microserviceName"></param>
        /// <param name="whiteLableRoute"></param>
        /// <param name="dbContextTypes"></param>
        /// <returns></returns>
        public virtual Task Initialize(string microserviceName, string whiteLableRoute, params Type[] dbContextTypes)
        {
            if (_service == null)
                throw new ObjectDisposedException(nameof(_service));
            return new WhiteLabelManager(_service).Initialize(microserviceName, whiteLableRoute, dbContextTypes);
        }

        IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, long> GetInternalLongContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract>()
          where TResponseContract : class
          where TEntity : class
        {
            return AddDisposable(new LongIdMappedDatabaseLogicBase<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract>(AddDisposable(GetDatabase().GetReadableOf<TEntity>()), AddDisposable(GetDatabase().GetWritableOf<TEntity>()), GetMapper(), GetUniqueIdentityManager()));
        }

        IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> GetInternalContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>()
          where TResponseContract : class
          where TEntity : class
        {
            return AddDisposable(new IdSchemaDatabaseMappedLogicBase<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>(AddDisposable(GetDatabase().GetReadableOf<TEntity>()), AddDisposable(GetDatabase().GetWritableOf<TEntity>()), GetMapper(), GetUniqueIdentityManager()));
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
            _service = null;
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
            _service = null;
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
    }
}
