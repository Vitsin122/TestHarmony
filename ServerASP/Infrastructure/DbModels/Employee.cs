using ServerASP.Infrastructure.DbContexts;

namespace ServerASP.Infrastructure.DbModels
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surename { get; set; } = null!;
        public int? Age { get ; set; }

        public int StatusId { get; set; }
        public virtual Status Status {get; set;} = null!;
    }
}
