using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.AspCoreApi;
/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TFilterRequestContract"></typeparam>
/// <typeparam name="TCreateRequestContract"></typeparam>
/// <typeparam name="TUpdateRequestContract"></typeparam>
/// <typeparam name="TResponseContract"></typeparam>
/// <typeparam name="TLanguageResponseContract"></typeparam>
[ApiController]
[Route("api/[controller]/[action]")]
public class MultilingualSimpleQueryServiceController<TEntity, TFilterRequestContract, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TLanguageResponseContract, TId> : MultilingualReadableQueryServiceController<TEntity, TFilterRequestContract, TResponseContract, TLanguageResponseContract, TId>
            where TResponseContract : class
            where TLanguageResponseContract : class
            where TEntity : class
            where TFilterRequestContract : FilterRequestContract
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <param name="contractLogic"></param>
    public MultilingualSimpleQueryServiceController(IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> contractLogic) : base(contractLogic)
    {
    }

    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <param name="unitOfWork"></param>
    public MultilingualSimpleQueryServiceController(IBaseUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.GetContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>())
    {
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> WritableContractLogic
    {
        get
        {
            return ContractLogic as IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>;
        }
    }

    string GetUniqueIdentity<T>(T item)
    {
        if (item is IUniqueIdentitySchema uniqueIdentitySchema)
            return uniqueIdentitySchema.UniqueIdentity;
        return (string)item.GetType()
            .GetProperty(nameof(IUniqueIdentitySchema.UniqueIdentity), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            ?.GetValue(item, null);
    }

    void SetUniqueIdentity<T>(T item, string value)
    {
        if (item is IUniqueIdentitySchema uniqueIdentitySchema)
            uniqueIdentitySchema.UniqueIdentity = value;
        else
            item.GetType()
                .GetProperty(nameof(IUniqueIdentitySchema.UniqueIdentity), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                ?.SetValue(item, value);
    }

    async Task<MessageContract<TId>> AddToContentLanguage(MessageContract<TId> messageContract, params object[] requests)
    {
        if (!messageContract)
            return messageContract;
        var contentResolver = UnitOfWork.GetContentResolver();
        var addedItem = await GetById(new Contracts.Requests.Multilingual.IdLanguageRequestContract<TId> { Id = messageContract })
                    .AsCheckedResult();
        foreach (var request in requests)
        {
            SetUniqueIdentity(request, GetUniqueIdentity(addedItem));
        }
        await contentResolver.AddToContentLanguage(requests);
        return messageContract;
    }

    async Task<ListMessageContract<TId>> AddToContentLanguage(ListMessageContract<TId> messageContract, params object[] requests)
    {
        if (!messageContract)
            return messageContract;
        var contentResolver = UnitOfWork.GetContentResolver();
        foreach (var item in messageContract.Result)
        {
            var addedItem = await GetById(new Contracts.Requests.Multilingual.IdLanguageRequestContract<TId> { Id = item })
                    .AsCheckedResult();
            foreach (var request in requests)
            {
                SetUniqueIdentity(request, GetUniqueIdentity(addedItem));
            }
        }
        await contentResolver.AddToContentLanguage(requests);
        return messageContract;
    }

    async Task<MessageContract<TResponseContract>> UpdateToContentLanguage(MessageContract<TResponseContract> messageContract, params object[] requests)
    {
        if (!messageContract)
            return messageContract;
        var contentResolver = UnitOfWork.GetContentResolver();
        foreach (var request in requests)
        {
            SetUniqueIdentity(request, GetUniqueIdentity(messageContract.Result));
        }
        await contentResolver.UpdateToContentLanguage(requests);
        return messageContract;
    }

    async Task<MessageContract> UpdateToContentLanguage(MessageContract messageContract, params object[] requests)
    {
        if (!messageContract)
            return messageContract;
        var contentResolver = UnitOfWork.GetContentResolver();
        await contentResolver.UpdateToContentLanguage(requests.Where(x=> GetUniqueIdentity(x).HasValue()));
        return messageContract;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<MessageContract<TId>> Add(TCreateRequestContract request, CancellationToken cancellationToken = default)
    {
        var result = await WritableContractLogic.Add(request, cancellationToken);
        return await AddToContentLanguage(result, request);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<MessageContract> AddBulk(CreateBulkRequestContract<TCreateRequestContract> request, CancellationToken cancellationToken = default)
    {
        var result = await WritableContractLogic.AddBulk(request, cancellationToken);
        return await AddToContentLanguage(result, request);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    public virtual async Task<MessageContract<TResponseContract>> Update(TUpdateRequestContract request, CancellationToken cancellationToken = default)
    {
        var result = await WritableContractLogic.Update(request, cancellationToken);
        return await UpdateToContentLanguage(result, request);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    public virtual async Task<MessageContract<TResponseContract>> UpdateChangedValuesOnly(TUpdateRequestContract request, CancellationToken cancellationToken = default)
    {
        var result = await WritableContractLogic.UpdateChangedValuesOnly(request, cancellationToken);
        return await UpdateToContentLanguage(result, request);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    public virtual async Task<MessageContract> UpdateBulk(UpdateBulkRequestContract<TUpdateRequestContract> request, CancellationToken cancellationToken = default)
    {
        var result = await WritableContractLogic.UpdateBulk(request, cancellationToken);
        return await UpdateToContentLanguage(result, request.Items);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    public virtual async Task<MessageContract> UpdateBulkChangedValuesOnly(UpdateBulkRequestContract<TUpdateRequestContract> request, CancellationToken cancellationToken = default)
    {
        var result = await WritableContractLogic.UpdateBulk(request, cancellationToken);
        return await UpdateToContentLanguage(result, request.Items);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    public virtual Task<MessageContract> HardDeleteById(DeleteRequestContract<TId> request, CancellationToken cancellationToken = default)
    {
        return WritableContractLogic.HardDeleteById(request, cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    public virtual Task<MessageContract> HardDeleteBulkByIds(DeleteBulkRequestContract<TId> request, CancellationToken cancellationToken = default)
    {
        return WritableContractLogic.HardDeleteBulkByIds(request, cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    public virtual Task<MessageContract> SoftDeleteById(SoftDeleteRequestContract<TId> request, CancellationToken cancellationToken = default)
    {
        return WritableContractLogic.SoftDeleteById(request, cancellationToken);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    public virtual Task<MessageContract> SoftDeleteBulkByIds(SoftDeleteBulkRequestContract<TId> request, CancellationToken cancellationToken = default)
    {
        return WritableContractLogic.SoftDeleteBulkByIds(request, cancellationToken);
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TCreateRequestContract"></typeparam>
/// <typeparam name="TUpdateRequestContract"></typeparam>
/// <typeparam name="TResponseContract"></typeparam>
/// <typeparam name="TLanguageResponseContract"></typeparam>
[ApiController]
[Route("api/[controller]/[action]")]
public class MultilingualSimpleQueryServiceController<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TLanguageResponseContract, TId> : MultilingualSimpleQueryServiceController<TEntity, FilterRequestContract, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TLanguageResponseContract, TId>
    where TResponseContract : class
    where TLanguageResponseContract : class
    where TEntity : class
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="contractLogic"></param>
    public MultilingualSimpleQueryServiceController(IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> contractLogic) : base(contractLogic)
    {
    }

    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <param name="unitOfWork"></param>
    public MultilingualSimpleQueryServiceController(IBaseUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}