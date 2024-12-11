using ClientWPF.Common;
using ClientWPF.Common.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Test_Client_Health_Hormony.Common.Commands;

namespace Test_Client_Health_Hormony
{
    public class EmployeeDtoRow : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }
        public int? Age { get; set; }
        public bool HasChanges { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class EmployeeContext : IDataErrorInfo, INotifyPropertyChanged
    {
        public MainWindowVM Parent;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }
        public string? Age { get; set; }

        public string Error => throw new NotImplementedException();

        public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
        public bool NoErrors { get; set; }

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "Name":
                        if (Name == null || Name == String.Empty)
                        {
                            error = "Имя должно быть заполнено";

                            if (!Errors.ContainsKey(columnName))
                            {
                                Errors[columnName] = error;
                                NoErrors = !Errors.Any();
                            }
                        }
                        else
                        {
                            if (Errors.ContainsKey(columnName))
                            {
                                Errors.Remove(columnName);
                                NoErrors = !Errors.Any();
                            }
                        }
                        break;
                    case "Surename":
                        if (Surename == null || Surename == String.Empty)
                        {
                            error = "Фамилия должна быть заполнена";

                            if (!Errors.ContainsKey(columnName))
                            {
                                Errors[columnName] = error;
                                NoErrors = !Errors.Any();
                            }
                        }
                        else
                        {
                            if (Errors.ContainsKey(columnName))
                            {
                                Errors.Remove(columnName);
                                NoErrors = !Errors.Any();
                            }
                        }
                        break;
                }
                return error;
            }
        }

        public EmployeeContext(MainWindowVM parent, EmployeeDtoRow employee)
        {
            Parent = parent;

            Id = employee.Id;
            Name = employee.Name;
            Surename = employee.Surename;
            Age = employee.Age?.ToString();

            NoErrors = !(Name == null || Name == String.Empty || Surename == null || Surename == String.Empty);

            CloseEditViewWindowCommand = Parent.CloseEditViewWindowCommand;
        }

        private RelayCommand<TextCompositionEventArgs> _ageChangedCommand;
        public RelayCommand<TextCompositionEventArgs> AgeChangedCommand => _ageChangedCommand ??= new RelayCommand<TextCompositionEventArgs>(AgeChanged);

        private RelayCommand _saveEmployeeCommand;
        public RelayCommand SaveEmployeeCommand => _saveEmployeeCommand ??= Parent.SaveEmployeeCommand;

        public RelayCommand CloseEditViewWindowCommand {  get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void AgeChanged(TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out int result);
        }
    }

    public class MainWindowVM : INotifyPropertyChanged
    {
        #region Fields
        private bool _isEditMode;
        private bool _isCreateEditWindowOpen;
        private EmployeeContext _employeeContext;
        #endregion

        #region Collections
        private ObservableCollection<EmployeeDto> Employees { get; set; } = new ObservableCollection<EmployeeDto>();
        public ObservableCollection<EmployeeDtoRow> EmployeesTemp { get; set; } = new ObservableCollection<EmployeeDtoRow>();
        #endregion

        #region Properties

        public bool IsEditMode { get => _isEditMode; set { _isEditMode = value; } }
        public bool IsCreateEditWindowOpen { get => _isCreateEditWindowOpen; set { _isCreateEditWindowOpen = value; } }
        public EmployeeContext EmployeeContext { get => _employeeContext; set { _employeeContext = value; } }
        public EmployeeDtoRow SelectedEmployee { get; set; }
        #endregion

        #region Commands

        private RelayCommand _loadedWindowCommand;
        public RelayCommand LoadedWindowCommand => _loadedWindowCommand ??= new RelayCommand(Loaded);


        private RelayCommand _setEditModeCommand;
        public RelayCommand SetEditModeCommand => _setEditModeCommand ??= new RelayCommand(SetEditMode);

        private RelayCommand _saveChangesCommand;
        public RelayCommand SaveChangesCommand => _saveChangesCommand ??= new RelayCommand(SaveChanges);

        private RelayCommand _openEditViewWindowCommand;
        public RelayCommand OpenEditViewWindowCommand => _openEditViewWindowCommand ??= new RelayCommand(OpenEditViewWindow);

        private RelayCommand _closeEditViewWindowCommand;
        public RelayCommand CloseEditViewWindowCommand => _closeEditViewWindowCommand ??= new RelayCommand(CloseEditViewWindow);

        private RelayCommand _saveEmployeeCommand;
        public RelayCommand SaveEmployeeCommand => _saveEmployeeCommand ??= new RelayCommand(SaveEmployee);

        private RelayCommand _discardChangesCommand;
        public RelayCommand DiscardChangesCommand => _discardChangesCommand ??= new RelayCommand(DiscardChanges);

        private RelayCommand _removeEmployeeCommand;
        public RelayCommand RemoveEmployeeCommand => _removeEmployeeCommand ??= new RelayCommand(RemoveEmployee);

        private RelayCommand _changeEmployeeCommand;
        public RelayCommand ChangeEmployeeCommand => _changeEmployeeCommand ??= new RelayCommand(ChangeEmployee);

        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region Methods
        public async void Loaded()
        {
            IsEditMode = false;

            await SetPropertiesViewModel();
        }

        private void SetEditMode()
        {
            IsEditMode = true;
        }

        private async Task SetPropertiesViewModel()
        {
            try
            {
                HttpResponseMessage allEmployeesResponse = await Client.GetAllEmployees();

                if (allEmployeesResponse.IsSuccessStatusCode)
                {
                    var result = await allEmployeesResponse.Content.ReadFromJsonAsync<List<EmployeeDto>>();

                    if (result == null)
                    {
                        Employees = new ObservableCollection<EmployeeDto>();
                    }
                    else
                    {
                        Employees = new ObservableCollection<EmployeeDto>(result);
                    }

                    EmployeesTemp = CastToTableCollection(Employees);
                }
                else
                {
                    var result = await allEmployeesResponse.Content.ReadAsStringAsync();

                    MessageBox.Show(result);
                }
            }
            catch
            {
                MessageBox.Show("Нет подключения к серверу...");
            }
        }

        private async void SaveChanges()
        {
            try
            {
                var updateResponse = await Client.UpdateRangeEmployees(CastToApiCollection(EmployeesTemp).ToList());

                if (updateResponse.IsSuccessStatusCode)
                {
                    await SetPropertiesViewModel();

                    SetViewMode();
                }
                else
                {
                    var result = await updateResponse.Content.ReadAsStringAsync();

                    MessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Нет подключения к серверу...");
            }

        }

        private void SaveEmployee()
        {
            var currentEmployee = EmployeesTemp.FirstOrDefault(e => e.Id == EmployeeContext.Id && EmployeeContext.Id != 0);

            if (currentEmployee != null)
            {
                currentEmployee.Name = EmployeeContext.Name;
                currentEmployee.Surename = EmployeeContext.Surename;
                currentEmployee.Age = EmployeeContext.Age == null || EmployeeContext.Age == String.Empty ? null : int.Parse(EmployeeContext.Age);
                currentEmployee.HasChanges = true;
            }
            else
            {
                var newEmployee = new EmployeeDtoRow
                {
                    Id = EmployeeContext.Id,
                    Name = EmployeeContext.Name,
                    Surename = EmployeeContext.Surename,
                    Age = EmployeeContext.Age == null || EmployeeContext.Age == String.Empty ? null : int.Parse(EmployeeContext.Age)
                };

                EmployeesTemp.Add(newEmployee);
            }

            CloseEditViewWindow();
        }

        private void CloseEditViewWindow()
        {
            IsCreateEditWindowOpen = false;
        }

        private void OpenEditViewWindow()
        {
            EmployeeContext = new EmployeeContext(this, new EmployeeDtoRow { Id = 0 });

            IsCreateEditWindowOpen = true;
        }

        private void SetViewMode()
        {
            IsEditMode = false;   
        }

        private void DiscardChanges()
        {
            EmployeesTemp = CastToTableCollection(Employees);

            SetViewMode();
        }

        private void RemoveEmployee()
        {
            EmployeesTemp.Remove(SelectedEmployee);
        }

        private void ChangeEmployee()
        {
            EmployeeContext = new EmployeeContext(this, new EmployeeDtoRow
            {
                Id = SelectedEmployee.Id,
                Name = SelectedEmployee.Name,
                Surename = SelectedEmployee.Surename,
                Age = SelectedEmployee.Age,
            });

            IsCreateEditWindowOpen = true;
        }

        private ObservableCollection<EmployeeDto> CastToApiCollection(ObservableCollection<EmployeeDtoRow> collection)
        {
            return new ObservableCollection<EmployeeDto>(collection.Select(x => new EmployeeDto
            {
                Id = x.Id,
                Name = x.Name,
                Surename = x.Surename,
                Age = x.Age,
                HasChanges = x.HasChanges,
            }));
        }


        private ObservableCollection<EmployeeDtoRow> CastToTableCollection(ObservableCollection<EmployeeDto> collection)
        {
            return new ObservableCollection<EmployeeDtoRow>(collection.Select(x => new EmployeeDtoRow
            {
                Id = x.Id,
                Name = x.Name,
                Surename = x.Surename,
                Age = x.Age,
                HasChanges = x.HasChanges,
            }));
        }
        #endregion
    }
}
