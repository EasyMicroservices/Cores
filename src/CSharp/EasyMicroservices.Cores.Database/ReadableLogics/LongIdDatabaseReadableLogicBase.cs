using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.Interfaces;
using ServiceContracts;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.ReadableLogics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class LongIdDatabaseReadableLogicBase<TEntity> : DatabaseReadableLogicBase, IReadableLogic<TEntity, long>
        where TEntity : class, IIdSchema<long>
    {
        readonly IEasyReadableQueryableAsync<TEntity> _easyReadableQueryable;
        public LongIdDatabaseReadableLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IMapperProvider mapperProvider) : base(mapperProvider)
        {
            _easyReadableQueryable = easyReadableQueryable;
        }

        public async Task<MessageContract<TEntity>> GetById(long id, CancellationToken cancellationToken = default)
        {
            return await GetById(_easyReadableQueryable, id, cancellationToken);
        }

        public async Task<MessageContract<List<TEntity>>> GetAll(CancellationToken cancellationToken = default)
        {
            return await GetAll(_easyReadableQueryable, cancellationToken);
        }
    }
}
