using ServerASP.BL.Dtos;

namespace ServerASP.BL.Interfaces
{
    /// <summary>
    /// Репозиторий по работе с сотрудниками
    /// </summary>
    public interface IEmployeeRepository
    {
        /// <summary>
        /// Получение сотрудника по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<EmployeeDto> GetById(int Id);

        /// <summary>
        /// Получение списка сотрудников
        /// </summary>
        /// <returns></returns>
        public Task<List<EmployeeDto>> GetEmployees();

        /// <summary>
        /// Создание нового сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task CreateEmployee(EmployeeDto model);

        /// <summary>
        /// Удаление сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteEmployee(int id);

        /// <summary>
        /// Обновление одного сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task UpdateEmployee(EmployeeDto model);

        /// <summary>
        /// Обновление полного списка сотрудников
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public Task UpdateEmployeeRange(List<EmployeeDto> models);
    }
}
