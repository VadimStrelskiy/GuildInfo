using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using GuildInfo.Wpf.Extentions;

namespace GuildInfo.Wpf.ViewModels
{
    public class CharacterListViewModel : ViewModelBase
    {
        private CharacterViewModel _selectedCharacter;
        private double _mainCharacterColumnWidth;
        private double _altsCountColumnWidth;

        public CharacterListViewModel(bool showMainCharacter, bool showAltsCount)
        {
            MainCharacterColumnWidth = showMainCharacter ? 100 : 0;
            AltsCountColumnWidth = showAltsCount ? 50 : 0;
        }

        public ObservableCollection<CharacterViewModel> Characters { get; } = new ObservableCollection<CharacterViewModel>();

        public CharacterViewModel SelectedCharacter
        {
            get { return _selectedCharacter; }
            set
            {
                if (value == _selectedCharacter) return;
                _selectedCharacter = value;
                OnPropertyChanged("SelectedCharacter");
            }
        }
        
        public void Clear()
        {
            Characters.Clear();
        }

        public void Add(CharacterViewModel character)
        {
            Characters.Add(character);
        }

        public void Add(IEnumerable<CharacterViewModel> character)
        {
            Characters.Add(character);
        }

        public double MainCharacterColumnWidth
        {
            get { return _mainCharacterColumnWidth; }
            set
            {
                _mainCharacterColumnWidth = value;
                OnPropertyChanged("MainCharacterColumnWidth");
            }
        }

        public double AltsCountColumnWidth
        {
            get { return _altsCountColumnWidth; }
            set
            {
                _altsCountColumnWidth = value;
                OnPropertyChanged("AltsCountColumnWidth");
            }
        }
    }
}
