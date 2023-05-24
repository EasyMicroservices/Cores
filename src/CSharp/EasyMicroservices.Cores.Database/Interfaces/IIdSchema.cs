namespace EasyMicroservices.Cores.Database.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IIdSchema<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        public TId Id { get; set; }
    }
}
