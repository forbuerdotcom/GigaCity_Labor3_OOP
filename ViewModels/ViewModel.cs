using System.ComponentModel;
using System.Runtime.CompilerServices;
using GigaCity_Labor3_OOP.Models;

namespace GigaCity_Labor3_OOP.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MapModel Map { get; private set; }

        private CellViewModel _selectedCell;
        public CellViewModel SelectedCell
        {
            get => _selectedCell;
            set
            {
                _selectedCell = value;
                OnPropertyChanged();
            }
        }

        private double _zoomLevel = 1.0;
        public double ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                _zoomLevel = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            Map = new MapModel();
            // Инициализируем пустую ячейку
            SelectedCell = new CellViewModel { X = 0, Y = 0, TerrainType = 1, ResourceType = 0 };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}