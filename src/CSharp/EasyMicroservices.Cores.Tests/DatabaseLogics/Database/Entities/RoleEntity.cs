namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities
{
    public class RoleEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ICollection<RoleParentChildEntity> Parents { get; set; }
        public ICollection<RoleParentChildEntity> Children { get; set; }
    }
}
