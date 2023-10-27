namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities
{
    public class RoleParentChildEntity
    {
        public long ParentId { get; set; }
        public long ChildId { get; set; }

        public RoleEntity Parent { get; set; }
        public RoleEntity Child { get; set; }
    }
}
