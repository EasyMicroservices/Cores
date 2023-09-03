namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities
{
    public class CompanyEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserCompanyEntity> UserCompanies { get; set; }
    }
}
