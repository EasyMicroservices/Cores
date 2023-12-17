using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace EasyMicroservices.Cores.AspCoreApi.Interfaces;
/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TFilterContract"></typeparam>
/// <typeparam name="TResponseContract"></typeparam>
/// <typeparam name="TIdRequestContract"></typeparam>
/// <typeparam name="TUniqueIdentityRequestContract"></typeparam>
/// <typeparam name="TId"></typeparam>
public interface IReadableQueryServiceController<TEntity, TFilterContract, TResponseContract, TId, TIdRequestContract, TUniqueIdentityRequestContract>
        where TResponseContract : class
        where TEntity : class
        where TFilterContract : FilterRequestContract
{
    /// <summary>
    /// 
    /// </summary>
    protected IContractReadableLogic<TEntity, TResponseContract, TId> ContractLogic { get; set; }

    /// <summary>
    /// 
    /// </summary>
    protected IBaseUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<MessageContract<TResponseContract>> GetById(TIdRequestContract request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<MessageContract<TResponseContract>> GetByUniqueIdentity(TUniqueIdentityRequestContract request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filterRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ListMessageContract<TResponseContract>> Filter(TFilterContract filterRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ListMessageContract<TResponseContract>> GetAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ListMessageContract<TResponseContract>> GetAllByUniqueIdentity(TUniqueIdentityRequestContract request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected Func<IQueryable<TEntity>, IQueryable<TEntity>> OnGetQuery();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected Func<IQueryable<TEntity>, IQueryable<TEntity>> OnGetAllQuery();
}
