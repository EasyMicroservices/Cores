using EasyMicroservices.Cores.AspCoreApi.Interfaces;
using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Contracts.Requests.Multilingual;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace EasyMicroservices.Cores.AspCoreApi;
/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TFilterContract"></typeparam>
/// <typeparam name="TResponseContract"></typeparam>
/// <typeparam name="TLanguageResponseContract"></typeparam>
/// <typeparam name="TId"></typeparam>
[ApiController]
[Route("api/[controller]/[action]")]
public class MultilingualReadableQueryServiceController<TEntity, TFilterContract, TResponseContract, TLanguageResponseContract, TId> :
    ControllerBase, IReadableQueryServiceController<TEntity, TFilterContract, TResponseContract, TId, GetByIdLanguageRequestContract<TId>, GetByUniqueIdentityLanguageRequestContract>
            where TResponseContract : class
            where TLanguageResponseContract : class
            where TEntity : class
            where TFilterContract : FilterRequestContract
{
    /// <summary>
    /// 
    /// </summary>
    public virtual IContractReadableLogic<TEntity, TResponseContract, TId> ContractLogic { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual IBaseUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="contractReadable"></param>
    public MultilingualReadableQueryServiceController(IContractReadableLogic<TEntity, TResponseContract, TId> contractReadable)
    {
        ContractLogic = contractReadable;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <param name="contractReadable"></param>
    public MultilingualReadableQueryServiceController(IBaseUnitOfWork unitOfWork, IContractReadableLogic<TEntity, TResponseContract, TId> contractReadable)
    {
        ContractLogic = contractReadable;
        UnitOfWork = unitOfWork;
    }
    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <param name="unitOfWork"></param>
    public MultilingualReadableQueryServiceController(IBaseUnitOfWork unitOfWork)
    {
        ContractLogic = unitOfWork.GetReadableContractLogic<TEntity, TResponseContract, TId>();
    }

    async Task<MessageContract<T>> ResolveContentLanguage<T>(MessageContract<T> result, string languageShortName)
    {
        if (!result)
            return result;
        var contentResolver = UnitOfWork.GetContentResolver();
        await contentResolver.ResolveContentLanguage(result.Result, languageShortName);
        return result;
    }

    async Task<ListMessageContract<T>> ResolveContentLanguage<T>(ListMessageContract<T> result, string languageShortName)
    {
        if (!result)
            return result;
        var contentResolver = UnitOfWork.GetContentResolver();
        await contentResolver.ResolveContentLanguage(result.Result, languageShortName);
        return result;
    }

    async Task<MessageContract<TLanguageResponseContract>> ResolveContentAllLanguage<T>(MessageContract<T> result)
    {
        if (!result)
            return result.ToContract<TLanguageResponseContract>();
        var contentResolver = UnitOfWork.GetContentResolver();
        var mapper = UnitOfWork.GetMapper();
        var mapped = mapper.Map<TLanguageResponseContract>(result.Result);
        await contentResolver.ResolveContentAllLanguage(mapped);
        return mapped;
    }

    async Task<ListMessageContract<TLanguageResponseContract>> ResolveContentAllLanguage<T>(ListMessageContract<T> result)
    {
        if (!result)
            return result.ToListContract<TLanguageResponseContract>();
        var contentResolver = UnitOfWork.GetContentResolver();
        var mapper = UnitOfWork.GetMapper();
        var mapped = mapper.MapToList<TLanguageResponseContract>(result.Result);
        await contentResolver.ResolveContentAllLanguage(mapped);
        return mapped;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<MessageContract<TResponseContract>> GetById(GetByIdLanguageRequestContract<TId> request, CancellationToken cancellationToken = default)
    {
        var result = await ContractLogic.GetById(request, OnGetQuery(), cancellationToken);
        return await ResolveContentLanguage(result, request.LanguageShortName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<MessageContract<TLanguageResponseContract>> GetByIdAllLanguage(GetByIdRequestContract<TId> request, CancellationToken cancellationToken = default)
    {
        var result = await ContractLogic.GetById(request, OnGetQuery(), cancellationToken);
        return await ResolveContentAllLanguage(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<MessageContract<TResponseContract>> GetByUniqueIdentity(GetByUniqueIdentityLanguageRequestContract request, CancellationToken cancellationToken = default)
    {
        var result = await ContractLogic.GetByUniqueIdentity(request, request.Type, OnGetQuery(), cancellationToken);
        return await ResolveContentLanguage(result, request.LanguageShortName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<MessageContract<TLanguageResponseContract>> GetByUniqueIdentityAllLanguage(GetByUniqueIdentityRequestContract request, CancellationToken cancellationToken = default)
    {
        var result = await ContractLogic.GetByUniqueIdentity(request, request.Type, OnGetQuery(), cancellationToken);
        return await ResolveContentAllLanguage(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filterRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<ListMessageContract<TResponseContract>> Filter(TFilterContract filterRequest, CancellationToken cancellationToken = default)
    {
        var result = await ContractLogic.Filter(filterRequest, OnGetAllQuery(), cancellationToken);
        return await ResolveContentLanguage(result, filterRequest.LanguageShortName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filterRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ListMessageContract<TLanguageResponseContract>> FilterAllLanguage(TFilterContract filterRequest, CancellationToken cancellationToken = default)
    {
        var result = await ContractLogic.Filter(filterRequest, OnGetAllQuery(), cancellationToken);
        return await ResolveContentAllLanguage(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public virtual async Task<ListMessageContract<TResponseContract>> GetAll(CancellationToken cancellationToken = default)
    {
        var result = await ContractLogic.GetAll(OnGetAllQuery(), cancellationToken);
        return await ResolveContentLanguage(result, default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="getByLanguageRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ListMessageContract<TResponseContract>> GetAllByLanguage(GetByLanguageRequestContract getByLanguageRequest, CancellationToken cancellationToken = default)
    {
        var result = await ContractLogic.GetAll(OnGetAllQuery(), cancellationToken);
        return await ResolveContentLanguage(result, getByLanguageRequest.LanguageShortName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ListMessageContract<TLanguageResponseContract>> GetAllWithAllLanguage(CancellationToken cancellationToken = default)
    {
        var result = await ContractLogic.GetAll(OnGetAllQuery(), cancellationToken);
        return await ResolveContentAllLanguage(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<ListMessageContract<TResponseContract>> GetAllByUniqueIdentity(GetByUniqueIdentityLanguageRequestContract request, CancellationToken cancellationToken = default)
    {
        var result = await ContractLogic.GetAllByUniqueIdentity(request, request.Type, OnGetAllQuery(), cancellationToken);
        return await ResolveContentLanguage(result, request.LanguageShortName);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ListMessageContract<TLanguageResponseContract>> GetAllByUniqueIdentityAllLanguage(GetByUniqueIdentityRequestContract request, CancellationToken cancellationToken = default)
    {
        var result = await ContractLogic.GetAllByUniqueIdentity(request, request.Type, OnGetAllQuery(), cancellationToken);
        return await ResolveContentAllLanguage(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual Func<IQueryable<TEntity>, IQueryable<TEntity>> OnGetQuery()
    {
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual Func<IQueryable<TEntity>, IQueryable<TEntity>> OnGetAllQuery()
    {
        return null;
    }
}


/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TResponseContract"></typeparam>
/// <typeparam name="TLanguageResponseContract"></typeparam>
/// <typeparam name="TId"></typeparam>
public class MultilingualReadableQueryServiceController<TEntity, TResponseContract, TLanguageResponseContract, TId>
    : MultilingualReadableQueryServiceController<TEntity, FilterRequestContract, TResponseContract, TLanguageResponseContract, TId>
            where TResponseContract : class
            where TLanguageResponseContract : class
            where TEntity : class
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="contractReadable"></param>
    public MultilingualReadableQueryServiceController(IContractReadableLogic<TEntity, TResponseContract, TId> contractReadable)
        : base(contractReadable)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <param name="contractReadable"></param>
    public MultilingualReadableQueryServiceController(IBaseUnitOfWork unitOfWork, IContractReadableLogic<TEntity, TResponseContract, TId> contractReadable)
        : base(unitOfWork, contractReadable)
    {

    }
    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <param name="unitOfWork"></param>
    public MultilingualReadableQueryServiceController(IBaseUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
        ContractLogic = unitOfWork.GetReadableContractLogic<TEntity, TResponseContract, TId>();
    }
}