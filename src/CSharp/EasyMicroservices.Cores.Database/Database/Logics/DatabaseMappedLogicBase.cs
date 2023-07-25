using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.Interfaces;
using EasyMicroservices.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Logics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TCreateRequestContract"></typeparam>
    /// <typeparam name="TUpdateRequestContract"></typeparam>
    /// <typeparam name="TResponseContract"></typeparam>
    public class DatabaseMappedLogicBase<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract> : DatabaseLogicInfrastructure, IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TResponseContract>
        where TEntity : class
        where TResponseContract : class
    {
        readonly IEasyReadableQueryableAsync<TEntity> _easyReadableQueryable;
        readonly IEasyWritableQueryableAsync<TEntity> _easyWriteableQueryable;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="mapperProvider"></param>
        /// <param name="uniqueIdentityManager"></param>
        public DatabaseMappedLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IMapperProvider mapperProvider, IUniqueIdentityManager uniqueIdentityManager) : base(mapperProvider, uniqueIdentityManager)
        {
            _easyReadableQueryable = easyReadableQueryable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="mapperProvider"></param>
        /// <param name="uniqueIdentityManager"></param>
        public DatabaseMappedLogicBase(IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IMapperProvider mapperProvider, IUniqueIdentityManager uniqueIdentityManager) : base(mapperProvider, uniqueIdentityManager)
        {
            _easyWriteableQueryable = easyWriteableQueryable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="mapperProvider"></param>
        /// <param name="uniqueIdentityManager"></param>
        public DatabaseMappedLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IMapperProvider mapperProvider, IUniqueIdentityManager uniqueIdentityManager) : base(mapperProvider, uniqueIdentityManager)
        {
            _easyWriteableQueryable = easyWriteableQueryable;
            _easyReadableQueryable = easyReadableQueryable;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<List<TResponseContract>>> GetAll(CancellationToken cancellationToken = default)
        {
            return await GetAll<TEntity, TResponseContract>(_easyReadableQueryable, null, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<MessageContract<TResponseContract>> GetById(TResponseContract contract, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default)
        {
            throw new Exception("GetById is not supported in DatabaseMappedLogicBase, you can use IdSchemaDatabaseMappedLogicBase or override this GetById method");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TResponseContract>> GetBy(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return await GetBy<TEntity, TResponseContract>(_easyReadableQueryable, predicate, func, cancellationToken);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TResponseContract>> Add(TCreateRequestContract contract, CancellationToken cancellationToken = default)
        {
            var result = await Add(_easyWriteableQueryable, contract, cancellationToken);
            if (!result)
                return result.ToContract<TResponseContract>();
            var mapped = await _mapperProvider.MapAsync<TResponseContract>(result.Result);
            ValidateMappedResult(ref mapped);
            return mapped;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<MessageContract<List<TResponseContract>>> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> query = null, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return await GetAll<TEntity, TResponseContract>(_easyReadableQueryable, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<TResponseContract>> GetById(TResponseContract contract, CancellationToken cancellationToken = default)
        {
            throw new Exception("GetById is not supported in DatabaseMappedLogicBase, you can use IdSchemaDatabaseMappedLogicBase or override this GetById method");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _easyWriteableQueryable.SaveChangesAsync(cancellationToken);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<MessageContract> HardDeleteBy(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return HardDeleteBy<TEntity, TResponseContract>(_easyWriteableQueryable, predicate, cancellationToken);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<TResponseContract>> Update(TUpdateRequestContract schema, CancellationToken cancellationToken = default)
        {
            return Update<TEntity, TUpdateRequestContract, TResponseContract>(_easyWriteableQueryable, schema, cancellationToken);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<MessageContract> HardDeleteById(TResponseContract contract, CancellationToken cancellationToken = default)
        {
            throw new Exception("HardDeleteById is not supported in DatabaseMappedLogicBase, you can use IdSchemaDatabaseMappedLogicBase or override this HardDeleteById method");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<TResponseContract>> GetByUniqueIdentity(IUniqueIdentitySchema request, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = null, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return base.GetByUniqueIdentity<TEntity, TResponseContract>(_easyReadableQueryable, request, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<List<TResponseContract>>> GetAllByUniqueIdentity(IUniqueIdentitySchema request, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = null, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return base.GetAllByUniqueIdentity<TEntity, TResponseContract>(_easyReadableQueryable, request, func, cancellationToken);
        }
    }
}
