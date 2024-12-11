namespace ServerASP.BL.Dtos
{
    /// <summary>
    /// Dto для клиента, чтобы не присылать ненужный StatusId
    /// </summary>
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }
        public int? Age { get; set; }

        /// <summary>
        /// Флаг, необходимый для опознания объекта как изменённого, чтобы его обновить
        /// </summary>
        public bool HasChanges { get; set; }
    }
}
