﻿using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Logics;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore;
using EasyMicroservices.Database.EntityFrameworkCore.Providers;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.CompileTimeMapper.Interfaces;
using EasyMicroservices.Mapper.CompileTimeMapper.Providers;
using EasyMicroservices.Mapper.Interfaces;
using EasyMicroservices.Mapper.SerializerMapper.Providers;
using EasyMicroservices.Serialization.Newtonsoft.Json.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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
        IServiceProvider _service;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public UnitOfWork(IServiceProvider service)
        {
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
        public IDatabase GetDatabase()
        {
            return AddDisposable(new EntityFrameworkCoreDatabaseProvider(_service.GetService<RelationalCoreContext>()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TCreateRequestContract"></typeparam>
        /// <typeparam name="TUpdateRequestContract"></typeparam>
        /// <typeparam name="TResponseContract"></typeparam>
        /// <returns></returns>
        public virtual IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, long> GetContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract>()
            where TEntity : class, IIdSchema<long>
            where TResponseContract : class
        {
            return AddDisposable(new LongIdMappedDatabaseLogicBase<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract>(GetDatabase().GetReadableOf<TEntity>(), GetDatabase().GetWritableOf<TEntity>(), GetMapper(), GetUniqueIdentityManager()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual IEasyReadableQueryableAsync<TEntity> GetReadableQueryable<TEntity>()
            where TEntity : class, IIdSchema<long>
        {
            return GetDatabase().GetReadableOf<TEntity>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IDatabase GetDatabase<TContext>()
                where TContext : RelationalCoreContext
        {
            return AddDisposable(new EntityFrameworkCoreDatabaseProvider(_service.GetService<TContext>()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IMapperProvider GetMapper()
        {
            var mapper = new CompileTimeMapperProvider(new SerializerMapperProvider(new NewtonsoftJsonProvider()));
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
        public static string DefaultUniqueIdentity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static long MicroserviceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static IUniqueIdentityManager UniqueIdentityManager { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IUniqueIdentityManager GetUniqueIdentityManager()
        {
            if (UniqueIdentityManager == null)
                UniqueIdentityManager = new DefaultUniqueIdentityManager(DefaultUniqueIdentity, MicroserviceId);
            return UniqueIdentityManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
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
        public async ValueTask DisposeAsync()
        {
            foreach (var item in Disposables)
            {
                if (item is IAsyncDisposable disposable)
                {
                    await disposable.DisposeAsync();
                }
            }
            Dispose();
        }
    }
}