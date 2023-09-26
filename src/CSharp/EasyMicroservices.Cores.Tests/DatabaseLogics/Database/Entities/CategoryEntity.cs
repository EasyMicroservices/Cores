using EasyMicroservices.Cores.Database.Schemas;

namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities
{
    public class CategoryEntity : FullAbilityIdSchema<long>
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public ICollection<CategoryEntity> Children { get; set; }
    }
}
