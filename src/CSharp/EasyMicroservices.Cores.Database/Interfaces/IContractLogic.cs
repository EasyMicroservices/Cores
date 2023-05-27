namespace EasyMicroservices.Cores.Database.Interfaces
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRequestContract"></typeparam>
    /// <typeparam name="TResponseContract"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IContractLogic<TEntity, TRequestContract, TResponseContract, TId> : IContractReadableLogic<TEntity, TResponseContract, TId>, IContractWritableLogic<TEntity, TRequestContract, TId>
    {

    }
}
