using Microsoft.EntityFrameworkCore;
using ServerASP.BL.Dtos;
using ServerASP.BL.Enums;
using ServerASP.Infrastructure.DbContexts;
using ServerASP.Infrastructure.DbModels;

namespace ServerASP.BL.Repositories
{
    public class EmployeeRepository
    {
        private MyContext _context;

        public EmployeeRepository(MyContext context)
        {
            _context = context;
        }

        public async Task<EmployeeDto> GetById(int Id)
        {
            try
            {
                var currentEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == Id && e.StatusId != (int)StatusEnum.Deleted);

                if (currentEmployee == null)
                {
                    throw new Exception("Сотрудник не найден...");
                }

                return new EmployeeDto() { Id = currentEmployee.Id, Name = currentEmployee.Name, Surename = currentEmployee.Surename};
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось получить сотрудника...");
            }
        }

        public async Task<List<EmployeeDto>> GetEmployees()
        {
            try
            {
                var employees = await _context.Employees.Where(e => e.StatusId != (int)StatusEnum.Deleted).ToListAsync();

                var result = employees.Select<Employee, EmployeeDto>(e => new EmployeeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Surename = e.Surename,
                    Age = e.Age,
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось прочитать данные о сотрудниках...");
            }
        }

        public async Task CreateEmployee(EmployeeDto model)
        {
            if (model == null)
                throw new Exception("Нельзя добавить сотрудника без данных...");

            if (model.Id != 0)
                throw new Exception("Нельзя добавить сотрудника с Id не равным 0...");

            if (model.Name == null || model.Surename == null || model.Name == String.Empty || model.Surename == String.Empty)
                throw new Exception("У сотрудника должны быть заполнены имя и фамилия...");

            try
            {
                var toAddEmployee = new Employee()
                {
                    Id = 0,
                    Name = model.Name,
                    Surename = model.Surename,
                    Age = model.Age,
                    StatusId = (int)StatusEnum.Active,
                };

                _context.Employees.Add(toAddEmployee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось записать данные о новом сотруднике...");
            }
        }

        public async Task DeleteEmployee(int id)
        {
            var currentEmployee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id && x.StatusId != (int)StatusEnum.Deleted);

            if (currentEmployee == null)
                throw new Exception("Сотрудник не найден...");

            currentEmployee.StatusId = (int)StatusEnum.Deleted;

            try
            {
                _context.Employees.Update(currentEmployee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                throw new Exception("Не удалось удалить сотрудника...");
            }
        }

        public async Task UpdateEmployee(EmployeeDto model)
        {
            var currentEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == model.Id && e.StatusId != (int)StatusEnum.Deleted);

            if (currentEmployee == null)
                throw new Exception("Сотрудник не найден...");

            try
            {
                currentEmployee.Name = model.Name;
                currentEmployee.Surename = model.Surename;
                currentEmployee.Age = model.Age;

                _context.Employees.Update(currentEmployee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось изменить сотрудника...");
            }
        }

        public async Task UpdateEmployeeRange(List<EmployeeDto> models)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var employees = await _context.Employees.Where(e => e.StatusId != (int)StatusEnum.Deleted).ToListAsync();

                    var addedEmployees = models.Where(m => !employees.Exists(e => e.Id == m.Id)).ToList();
                    var deletedEmployees = employees.Where(e => !models.Exists(m => m.Id == e.Id && m.Id != 0)).ToList();

                    if (deletedEmployees.Any())
                    {
                        foreach(var employee in deletedEmployees)
                        {
                            await DeleteEmployee(employee.Id);
                        }
                    }

                    if (addedEmployees.Any())
                    {
                        foreach (var employee in addedEmployees)
                        {
                            await CreateEmployee(employee);
                            employee.HasChanges = false;
                        }
                    }

                    var updatedEmployees = models.Where(m => m.HasChanges == true).ToList();

                    if (updatedEmployees.Any())
                    {
                        foreach (var employee in updatedEmployees)
                        {
                            await UpdateEmployee(employee);
                        }
                    }
                    
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Не удалось обновить сотрудников...");
                }
            }
        }
    }
}
