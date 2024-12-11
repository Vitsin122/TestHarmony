using ServerASP.Infrastructure.DbModels;

namespace ServerASP.Infrastructure.DbContexts
{
    public class Status
    {
        public int Id { get; set; }

        public string Description { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
