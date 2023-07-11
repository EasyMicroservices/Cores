using System;

namespace EasyMicroservices.Cores.Database.Interfaces
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TCreateRequestContract"></typeparam>
    /// <typeparam name="TResponseContract"></typeparam>
    /// <typeparam name="TUpdateRequestContract"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> : IContractReadableLogic<TEntity, TResponseContract, TId>, IContractWritableLogic<TEntity, TCreateRequestContract, TResponseContract, TUpdateRequestContract, TId>
        , IDisposable
#if (!NETSTANDARD2_0 && !NET45)
        , IAsyncDisposable
#endif
    {

    }
}
