namespace ServerASP.BL.Dtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }
        public int? Age { get; set; }
        public bool HasChanges { get; set; }
    }
}
