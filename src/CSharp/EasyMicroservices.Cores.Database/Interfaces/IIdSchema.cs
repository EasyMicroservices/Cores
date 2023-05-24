namespace EasyMicroservices.Cores.Database.Interfaces
{
    public interface IIdSchema<TId>
    {
        public TId Id { get; set; }
    }
}
