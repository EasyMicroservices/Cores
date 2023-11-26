using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.Interfaces;
using EasyMicroservices.ServiceContracts;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Logics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class LongIdDatabaseLogicBase<TEntity> : DatabaseLogicBase<TEntity, long>, IReadableLogic<TEntity, long>, IWritableLogic<TEntity, TEntity, long>
        where TEntity : class, IIdSchema<long>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="baseUnitOfWork"></param>
        public LongIdDatabaseLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IBaseUnitOfWork baseUnitOfWork) : base(easyReadableQueryable, baseUnitOfWork)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="baseUnitOfWork"></param>
        public LongIdDatabaseLogicBase(IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IBaseUnitOfWork baseUnitOfWork) : base(easyWriteableQueryable, baseUnitOfWork)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="baseUnitOfWork"></param>
        public LongIdDatabaseLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IBaseUnitOfWork baseUnitOfWork) : base(easyReadableQueryable, easyWriteableQueryable, baseUnitOfWork)
        {
        }
    }
}
