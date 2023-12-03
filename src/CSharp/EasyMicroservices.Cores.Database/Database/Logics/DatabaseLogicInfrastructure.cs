using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.DataTypes;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.Interfaces;
using EasyMicroservices.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Logics
{
    /// <summary>
    /// 
    /// </summary>
    public class DatabaseLogicInfrastructure : IDisposable
#if (!NETSTANDARD2_0 && !NET45)
        , IAsyncDisposable
#endif
    {
        /// <summary>
        /// 
        /// </summary>
        internal protected readonly IMapperProvider MapperProvider;
        readonly IBaseUnitOfWork _baseUnitOfWork;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUnitOfWork"></param>
        public DatabaseLogicInfrastructure(IBaseUnitOfWork baseUnitOfWork)
        {
            _baseUnitOfWork = baseUnitOfWork;
            MapperProvider = baseUnitOfWork.GetMapper();
        }

        async Task<IUniqueIdentityManager> GetIUniqueIdentityManager()
        {
            await _baseUnitOfWork.InitializeWhiteLabel();
            return _baseUnitOfWork.GetUniqueIdentityManager();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="NullReferenceException"></exception>
        protected virtual void ValidateMappedResult<T>(ref T value)
        {
            if (value == null || value.Equals(default(T)))
                throw new NullReferenceException("the result was null when we mapped it to contract! something went wrong!");
        }

        private async Task<IEasyReadableQueryableAsync<TEntity>> UniqueIdentityQueryMaker<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, string uniqueIdentity, GetUniqueIdentityType type)
            where TEntity : class
        {
            if (!typeof(IUniqueIdentitySchema).IsAssignableFrom(typeof(TEntity)))
                return easyReadableQueryable;
            var uniqueIdentityManager = await GetIUniqueIdentityManager();
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (!uniqueIdentityManager.IsUniqueIdentityForThisTable<TEntity>(easyReadableQueryable.Context, uniqueIdentity))
                uniqueIdentity += "-";
            bool isEndWithDash = uniqueIdentity.EndsWith("-");
            bool doEqual = false;
            if (type == GetUniqueIdentityType.OnlyParent)
            {
                var tableUniqueIdentity = uniqueIdentityManager.GetTableUniqueIdentity<TEntity>(easyReadableQueryable.Context);
                var lastTableUniqueIdentity = uniqueIdentityManager.GetLastTableUniqueIdentity(uniqueIdentity);
                doEqual = tableUniqueIdentity.Equals(lastTableUniqueIdentity);

                if (!isEndWithDash && !doEqual)
                    uniqueIdentity += "-";
                if (!doEqual)
                    uniqueIdentity += tableUniqueIdentity + "-";
            }
            else if (type == GetUniqueIdentityType.OnlyChilren)
            {
                if (!isEndWithDash)
                    uniqueIdentity += "-";
                uniqueIdentity += uniqueIdentityManager.GetLastTableUniqueIdentity(uniqueIdentity) + "-";
            }
            if (doEqual)
                queryable = queryable.ConvertToReadable(queryable.Where(x => (x as IUniqueIdentitySchema).UniqueIdentity.Equals(uniqueIdentity)));
            else
                queryable = queryable.ConvertToReadable(queryable.Where(x => (x as IUniqueIdentitySchema).UniqueIdentity.StartsWith(uniqueIdentity)));
            return queryable;
        }

        #region Get one

        /// <summary>
        /// get an item by an id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="idRequest"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<MessageContract<TEntity>> GetById<TEntity, TId>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, GetIdRequestContract<TId> idRequest, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (query != null)
                queryable = query(queryable);

            if (typeof(ISoftDeleteSchema).IsAssignableFrom(typeof(TEntity)))
                queryable = queryable.ConvertToReadable(queryable.Where(x => !(x as ISoftDeleteSchema).IsDeleted));

            if (typeof(IIdSchema<TId>).IsAssignableFrom(typeof(TEntity)))
                queryable = queryable.ConvertToReadable(queryable.Where(x => (x as IIdSchema<TId>).Id.Equals(idRequest.Id)));
            else
                return (FailedReasonType.OperationFailed, $"You cannot call GetById when your entity is not inheritance from IIdSchema<TId>! Please use it for {typeof(TEntity)}");

            var result = await queryable.FirstOrDefaultAsync(cancellationToken);
            if (result == null)
                return (FailedReasonType.NotFound, $"Item by id {idRequest.Id} not found!");
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="query"></param>
        /// <param name="doCheckIsDelete"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> GetBy<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Expression<Func<TEntity, bool>> predicate, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, bool doCheckIsDelete = true, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (query != null)
                queryable = query(queryable);

            if (doCheckIsDelete && typeof(ISoftDeleteSchema).IsAssignableFrom(typeof(TEntity)))
                queryable = queryable.ConvertToReadable(queryable.Where(x => !(x as ISoftDeleteSchema).IsDeleted));

            var result = await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
            if (result == null)
                return (FailedReasonType.NotFound, $"Item by predicate not found!");
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="query"></param>
        /// <param name="doCheckIsDelete"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ListMessageContract<TEntity>> GetAllBy<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Expression<Func<TEntity, bool>> predicate, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, bool doCheckIsDelete = true, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (query != null)
                queryable = query(queryable);

            if (doCheckIsDelete && typeof(ISoftDeleteSchema).IsAssignableFrom(typeof(TEntity)))
                queryable = queryable.ConvertToReadable(queryable.Where(x => !(x as ISoftDeleteSchema).IsDeleted));

            var result = await queryable.ConvertToReadable(queryable.Where(predicate)).ToListAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> GetBy<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (query != null)
                queryable = query(queryable);

            if (typeof(ISoftDeleteSchema).IsAssignableFrom(typeof(TEntity)))
                queryable = queryable.ConvertToReadable(queryable.Where(x => !(x as ISoftDeleteSchema).IsDeleted));

            var result = await queryable.FirstOrDefaultAsync(cancellationToken);
            if (result == null)
                return (FailedReasonType.NotFound, $"Item by predicate not found!");
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TContract>> GetBy<TEntity, TContract>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Expression<Func<TEntity, bool>> predicate, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
            where TContract : class
        {
            var entityResult = await GetBy(easyReadableQueryable, predicate, query, true, cancellationToken);
            if (!entityResult)
                return entityResult.ToContract<TContract>();
            var result = await MapAsync<TContract, TEntity>(entityResult.Result);
            return result;
        }

        /// <summary>
        /// get an item by an id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="idRequest"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<MessageContract<TContract>> GetById<TEntity, TContract, TId>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, GetIdRequestContract<TId> idRequest, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
            where TContract : class
        {
            var entityResult = await GetById(easyReadableQueryable, idRequest, query, cancellationToken);
            if (!entityResult)
                return entityResult.ToContract<TContract>();
            var result = await MapAsync<TContract, TEntity>(entityResult.Result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<MessageContract<TContract>> GetByUniqueIdentity<TEntity, TContract>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IUniqueIdentitySchema request, GetUniqueIdentityType type = GetUniqueIdentityType.All, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = null, CancellationToken cancellationToken = default)
            where TEntity : class
            where TContract : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = await UniqueIdentityQueryMaker(easyReadableQueryable, request.UniqueIdentity, type);
            var entityResult = await GetBy(queryable, query, cancellationToken);
            if (!entityResult)
                return entityResult.ToContract<TContract>();
            var result = await MapAsync<TContract, TEntity>(entityResult.Result);
            return result;
        }


        #endregion

        #region Get list

        /// <summary>
        /// get all items
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filterRequest"></param>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<ListMessageContract<TEntity>> Filter<TEntity>(FilterRequestContract filterRequest, IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (query != null)
                queryable = query(queryable);
            var countQueryable = queryable;
            if (filterRequest.Index.HasValue)
                queryable = queryable.ConvertToReadable(queryable.Skip((int)filterRequest.Index.Value));
            if (filterRequest.Length.HasValue)
                queryable = queryable.ConvertToReadable(queryable.Take((int)filterRequest.Length.Value));

            if (filterRequest.UniqueIdentity.HasValue() && typeof(IUniqueIdentitySchema).IsAssignableFrom(typeof(TEntity)))
            {
                queryable = await UniqueIdentityQueryMaker(easyReadableQueryable, filterRequest.UniqueIdentity, filterRequest.UniqueIdentityType.HasValue ? filterRequest.UniqueIdentityType.Value : GetUniqueIdentityType.All);
                countQueryable = await UniqueIdentityQueryMaker(easyReadableQueryable, filterRequest.UniqueIdentity, filterRequest.UniqueIdentityType.HasValue ? filterRequest.UniqueIdentityType.Value : GetUniqueIdentityType.All);
            }
            if (filterRequest.FromCreationDateTime.HasValue && typeof(IDateTimeSchema).IsAssignableFrom(typeof(TEntity)))
            {
                queryable = queryable.ConvertToReadable(queryable.Where(x => (x as IDateTimeSchema).CreationDateTime >= filterRequest.FromCreationDateTime));
                countQueryable = countQueryable.ConvertToReadable(countQueryable.Where(x => (x as IDateTimeSchema).CreationDateTime >= filterRequest.FromCreationDateTime));
            }
            if (filterRequest.ToCreationDateTime.HasValue && typeof(IDateTimeSchema).IsAssignableFrom(typeof(TEntity)))
            {
                queryable = queryable.ConvertToReadable(queryable.Where(x => (x as IDateTimeSchema).CreationDateTime <= filterRequest.ToCreationDateTime));
                countQueryable = countQueryable.ConvertToReadable(countQueryable.Where(x => (x as IDateTimeSchema).CreationDateTime <= filterRequest.ToCreationDateTime));
            }
            if (filterRequest.FromModificationDateTime.HasValue && typeof(IDateTimeSchema).IsAssignableFrom(typeof(TEntity)))
            {
                queryable = queryable.ConvertToReadable(queryable.Where(x => (x as IDateTimeSchema).ModificationDateTime >= filterRequest.FromModificationDateTime));
                countQueryable = countQueryable.ConvertToReadable(countQueryable.Where(x => (x as IDateTimeSchema).ModificationDateTime >= filterRequest.FromModificationDateTime));
            }
            if (filterRequest.ToModificationDateTime.HasValue && typeof(IDateTimeSchema).IsAssignableFrom(typeof(TEntity)))
            {
                queryable = queryable.ConvertToReadable(queryable.Where(x => (x as IDateTimeSchema).ModificationDateTime <= filterRequest.ToModificationDateTime));
                countQueryable = countQueryable.ConvertToReadable(countQueryable.Where(x => (x as IDateTimeSchema).ModificationDateTime <= filterRequest.ToModificationDateTime));
            }
            if (filterRequest.FromDeletedDateTime.HasValue && typeof(ISoftDeleteSchema).IsAssignableFrom(typeof(TEntity)))
            {
                queryable = queryable.ConvertToReadable(queryable.Where(x => (x as ISoftDeleteSchema).DeletedDateTime >= filterRequest.FromDeletedDateTime));
                countQueryable = countQueryable.ConvertToReadable(countQueryable.Where(x => (x as ISoftDeleteSchema).DeletedDateTime >= filterRequest.FromDeletedDateTime));
            }
            if (filterRequest.ToDeletedDateTime.HasValue && typeof(ISoftDeleteSchema).IsAssignableFrom(typeof(TEntity)))
            {
                queryable = queryable.ConvertToReadable(queryable.Where(x => (x as ISoftDeleteSchema).DeletedDateTime <= filterRequest.ToDeletedDateTime));
                countQueryable = countQueryable.ConvertToReadable(countQueryable.Where(x => (x as ISoftDeleteSchema).DeletedDateTime <= filterRequest.ToDeletedDateTime));
            }
            if (filterRequest.IsDeleted.HasValue && typeof(ISoftDeleteSchema).IsAssignableFrom(typeof(TEntity)))
            {
                queryable = queryable.ConvertToReadable(queryable.Where(x => (x as ISoftDeleteSchema).IsDeleted == filterRequest.IsDeleted));
                countQueryable = countQueryable.ConvertToReadable(countQueryable.Where(x => (x as ISoftDeleteSchema).IsDeleted == filterRequest.IsDeleted));
            }
            ListMessageContract<TEntity> result = await queryable.ToListAsync(cancellationToken);
            result.TotalCount = await countQueryable.CountAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filterRequest"></param>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<ListMessageContract<TContract>> Filter<TEntity, TContract>(FilterRequestContract filterRequest, IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
            where TContract : class
        {
            var entityResult = await Filter(filterRequest, easyReadableQueryable, query, cancellationToken);
            if (!entityResult)
                return entityResult.ToAnotherListContract<TContract>();
            var result = (ListMessageContract<TContract>)await MapToListAsync<TContract, TEntity>(entityResult.Result);
            result.TotalCount = entityResult.TotalCount;
            return result;
        }

        /// <summary>
        /// get all items
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<ListMessageContract<TEntity>> GetAll<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (query != null)
                queryable = query(queryable);

            if (typeof(ISoftDeleteSchema).IsAssignableFrom(typeof(TEntity)))
                queryable = queryable.ConvertToReadable(queryable.Where(x => !(x as ISoftDeleteSchema).IsDeleted));

            return await queryable.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// get all items mapped
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<ListMessageContract<TContract>> GetAll<TEntity, TContract>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
            where TContract : class
        {
            var entityResult = await GetAll(easyReadableQueryable, query, cancellationToken);
            if (!entityResult)
                return entityResult.ToAnotherListContract<TContract>();
            var result = await MapToListAsync<TContract, TEntity>(entityResult.Result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ListMessageContract<TContract>> GetAllByUniqueIdentity<TEntity, TContract>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IUniqueIdentitySchema request, GetUniqueIdentityType type = GetUniqueIdentityType.All, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = null, CancellationToken cancellationToken = default)
            where TEntity : class
            where TContract : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = await UniqueIdentityQueryMaker(easyReadableQueryable, request.UniqueIdentity, type);
            var entityResult = await GetAll(queryable, query, cancellationToken);
            if (!entityResult)
                return entityResult.ToAnotherListContract<TContract>();
            var result = await MapToListAsync<TContract, TEntity>(entityResult.Result);
            return result;
        }

        #endregion

        #region Update


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="entity"></param>
        /// <param name="updateOnlyChangedValue"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<TEntity>> Update<TEntity>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TEntity entity, bool updateOnlyChangedValue, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return InternalUpdate(easyWritableQueryable, entity, updateOnlyChangedValue, false, true, cancellationToken);
        }

        private async Task<MessageContract<TEntity>> InternalUpdate<TEntity>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TEntity entity, bool updateOnlyChangedValue, bool doSkipUpdate = true, bool doSkipDelete = true, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = await InternalUpdateBulk(easyWritableQueryable, new List<TEntity>() { entity }, updateOnlyChangedValue, doSkipUpdate, doSkipDelete, cancellationToken);
            if (!result)
                return result.ToContract<TEntity>();
            return result.Result.First();
        }

        private async Task<ListMessageContract<TEntity>> InternalUpdateBulk<TEntity>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, List<TEntity> entities, bool updateOnlyChangedValue, bool doSkipUpdate = true, bool doSkipDelete = true, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            List<IEntityEntry> items = new List<IEntityEntry>();
            var result = await easyWritableQueryable.UpdateBulkAsync(entities, cancellationToken);

            foreach (var entityEntry in easyWritableQueryable.Context.GetTrackerEntities().ToArray())
            {
                if (entityEntry.EntityState != EasyMicroservices.Database.DataTypes.EntityStateType.Modified
                    && entityEntry.EntityState != EasyMicroservices.Database.DataTypes.EntityStateType.Deleted)
                    continue;
                if (updateOnlyChangedValue)
                    UpdateOnlyChangedValue(easyWritableQueryable.Context, entityEntry.Entity);
                if (entityEntry.Entity is IDateTimeSchema schema)
                {
                    easyWritableQueryable.Context.ChangeModificationPropertyState(entityEntry.Entity, nameof(IDateTimeSchema.CreationDateTime), false);
                    if (!doSkipUpdate)
                        schema.ModificationDateTime = DateTime.Now;
                }
                if (doSkipDelete && entityEntry.Entity is ISoftDeleteSchema)
                {
                    easyWritableQueryable.Context.ChangeModificationPropertyState(entityEntry.Entity, nameof(ISoftDeleteSchema.DeletedDateTime), false);
                    easyWritableQueryable.Context.ChangeModificationPropertyState(entityEntry.Entity, nameof(ISoftDeleteSchema.IsDeleted), false);
                }
                items.Add(entityEntry);
            }
            await easyWritableQueryable.SaveChangesAsync();
            foreach (var entity in items)
                await easyWritableQueryable.Context.Reload(entity.Entity, cancellationToken);
            return items.Where(x => x.Entity is TEntity).Select(x => x.Entity).Cast<TEntity>().ToList();
        }

        void UpdateOnlyChangedValue<TEntity>(IContext context, TEntity entity)
            where TEntity : class
        {
            var properties = context.GetProperties(entity);
            foreach (var property in properties)
            {
                if (property.IsModified)
                {
                    if (property.CurrentValue is null && property.CurrentValue == GetDefault(property.Metadata.ClrType))
                        property.IsModified = false;
                    else if (property.CurrentValue.Equals(GetDefault(property.Metadata.ClrType)))
                        property.IsModified = false;
                }
            }
        }

        static object GetDefault(Type type)
        {
            return typeof(DatabaseLogicInfrastructure)
                .GetMethod(nameof(GetDefaultValueGeneric), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .MakeGenericMethod(type).Invoke(null, null);
        }

        static T GetDefaultValueGeneric<T>()
        {
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TUpdateContract"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="contract"></param>
        /// <param name="updateOnlyChangedValue"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TContract>> Update<TEntity, TUpdateContract, TContract>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TUpdateContract contract, bool updateOnlyChangedValue, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entity = await MapAsync<TEntity, TUpdateContract>(contract);
            var result = await Update(easyWritableQueryable, entity, updateOnlyChangedValue, cancellationToken);
            if (!result)
                return result.ToContract<TContract>();
            var mappedResult = await MapAsync<TContract, TEntity>(result.Result);
            return mappedResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TUpdateContract"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="request"></param>
        /// <param name="updateOnlyChangedValue"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> UpdateBulk<TEntity, TUpdateContract>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, UpdateBulkRequestContract<TUpdateContract> request, bool updateOnlyChangedValue, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entities = await MapToListAsync<TEntity, TUpdateContract>(request.Items);
            var result = await InternalUpdateBulk(easyWritableQueryable, entities, updateOnlyChangedValue, false, true, cancellationToken);
            return result;
        }
        #endregion

        #region Delete

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="deleteRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> HardDeleteById<TEntity, TId>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, DeleteRequestContract<TId> deleteRequest, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (!typeof(IIdSchema<TId>).IsAssignableFrom(typeof(TEntity)))
                return (FailedReasonType.OperationFailed, $"You cannot call GetById when your entity is not inheritance from IIdSchema<TId>! Please use it for {typeof(TEntity)}");

            var result = await easyWritableQueryable.RemoveAllAsync(x => ((IIdSchema<TId>)x).Id.Equals(deleteRequest.Id), cancellationToken);
            await easyWritableQueryable.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="deleteRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> HardDeleteById<TEntity, TContract, TId>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, DeleteRequestContract<TId> deleteRequest, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (!typeof(IIdSchema<TId>).IsAssignableFrom(typeof(TEntity)))
                return (FailedReasonType.OperationFailed, $"You cannot call GetById when your entity is not inheritance from IIdSchema<TId>! Please use it for {typeof(TEntity)}");

            var result = await easyWritableQueryable.RemoveAllAsync(x => ((IIdSchema<TId>)x).Id.Equals(deleteRequest.Id), cancellationToken);
            await easyWritableQueryable.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="deleteRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> HardDeleteBulkByIds<TEntity, TId>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, DeleteBulkRequestContract<TId> deleteRequest, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (!typeof(IIdSchema<TId>).IsAssignableFrom(typeof(TEntity)))
                return (FailedReasonType.OperationFailed, $"You cannot call GetById when your entity is not inheritance from IIdSchema<TId>! Please use it for {typeof(TEntity)}");

            var result = await easyWritableQueryable.RemoveAllAsync(x => deleteRequest.Ids.Contains(((IIdSchema<TId>)x).Id), cancellationToken);
            await easyWritableQueryable.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> HardDeleteBy<TEntity>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = await easyWritableQueryable.RemoveAllAsync(predicate, cancellationToken);
            await easyWritableQueryable.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> HardDeleteBy<TEntity, TContract>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
           where TEntity : class
        {
            var result = await easyWritableQueryable.RemoveAllAsync(predicate, cancellationToken);
            await easyWritableQueryable.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="deleteRequest"></param>
        /// /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> SoftDeleteById<TEntity, TId>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, SoftDeleteRequestContract<TId> deleteRequest, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (!typeof(IIdSchema<TId>).IsAssignableFrom(typeof(TEntity)))
                return (FailedReasonType.OperationFailed, $"You cannot call GetById when your entity is not inheritance from IIdSchema<TId>! Please use it for {typeof(TEntity)}");

            return await SoftDeleteBy(easyReadableQueryable, easyWritableQueryable, x => ((IIdSchema<TId>)x).Id.Equals(deleteRequest.Id), deleteRequest.IsDelete, query, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="deleteRequest"></param>
        /// /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> SoftDeleteBulkByIds<TEntity, TId>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, SoftDeleteBulkRequestContract<TId> deleteRequest, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (!typeof(IIdSchema<TId>).IsAssignableFrom(typeof(TEntity)))
                return (FailedReasonType.OperationFailed, $"You cannot call GetById when your entity is not inheritance from IIdSchema<TId>! Please use it for {typeof(TEntity)}");

            return await SoftDeleteBy(easyReadableQueryable, easyWritableQueryable, x => deleteRequest.Ids.Contains(((IIdSchema<TId>)x).Id), deleteRequest.IsDelete, query, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="isDelete"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> SoftDeleteBy<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, Expression<Func<TEntity, bool>> predicate, bool isDelete, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var getResult = await GetAllBy(easyReadableQueryable, predicate, query, false, cancellationToken);
            if (!getResult)
                return getResult;
            foreach (var item in getResult.Result)
            {
                if (item is ISoftDeleteSchema softDeleteSchema)
                {
                    softDeleteSchema.IsDeleted = isDelete;
                    if (isDelete)
                        softDeleteSchema.DeletedDateTime = DateTime.Now;
                    else
                        softDeleteSchema.DeletedDateTime = null;
                }
                else
                    return (FailedReasonType.OperationFailed, $"Your entity type {item.GetType().FullName} is not inheritance from ISoftDeleteSchema");
            }
            return await InternalUpdateBulk(easyWritableQueryable, getResult.Result, false, true, false, cancellationToken);
        }

        #endregion

        #region Add

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> Add<TEntity>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = await easyWritableQueryable.AddAsync(entity, cancellationToken);
            foreach (var entityEntry in easyWritableQueryable.Context.GetTrackerEntities().ToArray())
            {
                if (entityEntry.EntityState != EasyMicroservices.Database.DataTypes.EntityStateType.Added)
                    continue;
                if (entityEntry.Entity is IDateTimeSchema schema)
                {
                    schema.CreationDateTime = DateTime.Now;
                }
            }
            await easyWritableQueryable.SaveChangesAsync();
            if (typeof(IUniqueIdentitySchema).IsAssignableFrom(typeof(TEntity)))
            {
                var uniqueIdentityManager = await GetIUniqueIdentityManager();
                if (uniqueIdentityManager.UpdateUniqueIdentity(easyWritableQueryable.Context, result.Entity))
                {
                    await InternalUpdate(easyWritableQueryable, result.Entity, false, true, true, cancellationToken);
                    await easyWritableQueryable.SaveChangesAsync();
                }
            }

            return result.Entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> AddBulk<TEntity>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = await easyWritableQueryable.AddBulkAsync(entities, cancellationToken);
            foreach (var entityEntry in easyWritableQueryable.Context.GetTrackerEntities().ToArray())
            {
                if (entityEntry.EntityState != EasyMicroservices.Database.DataTypes.EntityStateType.Added)
                    continue;
                if (entityEntry.Entity is IDateTimeSchema schema)
                {
                    schema.CreationDateTime = DateTime.Now;
                }
            }
            await easyWritableQueryable.SaveChangesAsync();
            bool anyUpdate = false;
            if (typeof(IUniqueIdentitySchema).IsAssignableFrom(typeof(TEntity)))
            {
                var uniqueIdentityManager = await GetIUniqueIdentityManager();
                foreach (var item in result)
                {
                    if (uniqueIdentityManager.UpdateUniqueIdentity(easyWritableQueryable.Context, item.Entity))
                    {
                        anyUpdate = true;
                    }
                }
            }
            if (anyUpdate)
            {
                await InternalUpdateBulk(easyWritableQueryable, result.Select(x => x.Entity).ToList(), false, true, true, cancellationToken);
                await easyWritableQueryable.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="contract"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> Add<TEntity, TContract>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TContract contract, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entity = await MapAsync<TEntity, TContract>(contract);
            var result = await Add<TEntity>(easyWritableQueryable, entity, cancellationToken);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> AddBulk<TEntity, TContract>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, CreateBulkRequestContract<TContract> request, CancellationToken cancellationToken = default)
           where TEntity : class
        {
            var entities = await MapToListAsync<TEntity, TContract>(request.Items);
            var result = await AddBulk(easyWritableQueryable, entities, cancellationToken);
            return result;
        }

        #endregion

        #region Map

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TTo"></typeparam>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        protected async Task<List<TTo>> MapToListAsync<TTo, TFrom>(IEnumerable<TFrom> items)
        {
            if (typeof(TFrom) == typeof(TTo))
                return items.Cast<TTo>().ToList();
            var result = await MapperProvider.MapToListAsync<TTo>(items);
            ValidateMappedResult(ref result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TTo"></typeparam>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        protected async Task<TTo> MapAsync<TTo, TFrom>(TFrom item)
        {
            if (typeof(TFrom) == typeof(TTo))
                return (TTo)(object)item;
            var result = await MapperProvider.MapAsync<TTo>(item);
            ValidateMappedResult(ref result);
            return result;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {

        }

#if (!NETSTANDARD2_0 && !NET45)
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }
#endif
    }
}
