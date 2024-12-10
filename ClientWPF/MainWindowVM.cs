using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Test_Client_Health_Hormony.Common.Commands;

namespace Test_Client_Health_Hormony
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        #region Fields
        private bool _isEditMode;
        #endregion

        #region Properties

        public bool IsEditMode {get => _isEditMode; set { _isEditMode = value;} }
        #endregion

        #region Commands

        private RelayCommand _loadedWindowCommand;
        public RelayCommand LoadedWindowCommand => _loadedWindowCommand ??= new RelayCommand(Loaded);


        private RelayCommand _setEditModeCommand;
        public RelayCommand SetEditModeCommand => _setEditModeCommand ??= new RelayCommand(SetEditMode);

        private RelayCommand _saveChangesCommand;
        public RelayCommand SaveChangesCommand => _saveChangesCommand ??= new RelayCommand(SaveChanges);

        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region Methods
        public void Loaded()
        {
            _isEditMode = false;
        }
        
        private void SetEditMode()
        {
            IsEditMode = true;
        }

        private void SaveChanges()
        {
            SetViewMode();
        }

        private void SetViewMode()
        {
            IsEditMode = false;   
        }
        #endregion
    }
}
