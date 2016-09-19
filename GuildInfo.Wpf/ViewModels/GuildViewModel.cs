using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using GuildInfo.Core;
using GuildInfo.Core.Models;
using GuildInfo.Wpf.Selection;

namespace GuildInfo.Wpf.ViewModels
{
    public class GuildViewModel : ViewModelBase
    {
        private string _altsOf;
        private double _progress;
        private int _charactersLoaded;
        private bool? _allClassesSelected;
        private const string _altsOfNoone = "Alts of";

        private IList<int> _calculatedRanksToExclude;
        private IList<AggergatedCharacter> _initialCharacters;

        private readonly Configuration _config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

        public static RoutedCommand FetchCommand = new RoutedCommand("Fetch", typeof(GuildViewModel));
        public static RoutedCommand FilterCommand = new RoutedCommand("Filter", typeof(GuildViewModel));

        public GuildViewModel()
        {
            AltsOf = _altsOfNoone;
            MainCharacters.PropertyChanged += MainCharacters_PropertyChanged;

            var classes = new List<ClassViewModel>();
            for(int i = 1; i <= 12; i++)
            {
                classes.Add(new ClassViewModel(i, $"/GuildInfo.Wpf;component/Images/{i}.jpg"));
            }
            Classes = new SelectionList<ClassViewModel>(classes);

            Classes.PropertyChanged += Classes_PropertyChanged;
            AllClassesSelected = true;
        }

        private void Classes_PropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (Classes.All(c => c.IsSelected))
            {
                AllClassesSelected = true;
            }
            else if (Classes.All(c => !c.IsSelected))
            {
                AllClassesSelected = false;
            }
            else
            {
                AllClassesSelected = null;
            }
        }

        private void MainCharacters_PropertyChanged(object o, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedCharacter") return;

            AltCharacters.Clear();
            if (MainCharacters.SelectedCharacter == null)
            {
                AltsOf = _altsOfNoone;
                return;
            }

            AltCharacters.Add(MainCharacters.SelectedCharacter.Alts);
            AltsOf = $"Alts of {MainCharacters.SelectedCharacter.Name}:";
        }

        public CharacterListViewModel MainCharacters { get; } = new CharacterListViewModel(false, true);
        public CharacterListViewModel AltCharacters { get; } = new CharacterListViewModel(false, false);
        public CharacterListViewModel AllAltCharacters { get; } = new CharacterListViewModel(true, false);
        public CharacterListViewModel TopAltCharacters { get; } = new CharacterListViewModel(true, false);

        public SelectionList<ClassViewModel> Classes { get; }

        public string Realm
        {
            get { return GetConfigValue(nameof(Realm)); }
            set
            {
                SetConfigValue(nameof(Realm), value);
                OnPropertyChanged(nameof(Realm));
            }
        }

        public string Guild
        {
            get { return GetConfigValue(nameof(Guild)); }
            set
            {
                SetConfigValue(nameof(Guild), value);
                OnPropertyChanged(nameof(Guild));
            }
        }

        public int? MinItemLevel
        {
            get
            {
                int value;
                int.TryParse(GetConfigValue(nameof(MinItemLevel)), out value);
                return value;
            }
            set
            {
                SetConfigValue(nameof(MinItemLevel), value?.ToString());
                OnPropertyChanged(nameof(MinItemLevel));
            }
        }

        public int? MinLevel
        {
            get
            {
                int value;
                int.TryParse(GetConfigValue(nameof(MinLevel)), out value);
                return value;
            }
            set
            {
                SetConfigValue(nameof(MinLevel), value?.ToString());
                OnPropertyChanged(nameof(MinLevel));
            }
        }

        public string RanksToExclude
        {
            get { return GetConfigValue(nameof(RanksToExclude)); }
            set
            {
                SetConfigValue(nameof(RanksToExclude), value);
                OnPropertyChanged(nameof(RanksToExclude));
            }
        }

        public string AltsOf
        {
            get { return _altsOf; }
            set
            {
                if (value == _altsOf) return;
                _altsOf = value;
                OnPropertyChanged("AltsOf");
            }
        }

        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public bool? AllClassesSelected
        {
            get { return _allClassesSelected; }
            set
            {
                _allClassesSelected = value;
                if (_allClassesSelected == true)
                {
                    foreach (var @class in Classes)
                    {
                        @class.IsSelected = true;
                    }
                }
                else if(_allClassesSelected == false)
                {
                    foreach (var @class in Classes)
                    {
                        @class.IsSelected = false;
                    }
                }
                OnPropertyChanged("AllClassesSelected");
            }
        }

        public async Task Fetch()
        {
            _charactersLoaded = 0;
            Progress = 0;
            var service = new GuildService(ConfigurationManager.AppSettings["ApiKey"]);
            
            service.CharacterLoaded += (o, e) =>
            {
                _charactersLoaded ++;
                Progress = (double)_charactersLoaded/e.CharactersTotal;
            };
            var task = Task<IEnumerable<AggergatedCharacter>>.Factory.StartNew(() => service.Fetch(Realm, Guild));
            await task;

            _initialCharacters = task.Result.ToList();
            Filter();
        }

        public bool CanFetch
        {
            get { return !string.IsNullOrWhiteSpace(Realm) && !string.IsNullOrWhiteSpace(Guild); }
        }

        public void Filter()
        {
            CalculateRanksToExclude();
            ClearAll();

            var mainCharacters = new List<CharacterViewModel>();
            var topAltCharacters = new List<CharacterViewModel>();

            foreach (var character in _initialCharacters)
            {
                var filteredAndSortedAlts =
                    character.Alts
                    .Where(FilterCharacter)
                    .OrderByDescending(c => c.AverageItemLevel)
                    .Select(a => new CharacterViewModel(a, character.Main.Name))
                    .ToList();

                if (FilterCharacter(character.Main))
                {
                    mainCharacters.Add(new CharacterViewModel(character.Main, filteredAndSortedAlts));
                }

                var topAltCharacter = filteredAndSortedAlts.FirstOrDefault();
                if (topAltCharacter != null)
                {
                    topAltCharacters.Add(topAltCharacter);
                }
            }

            MainCharacters.Add(ApplyInitialOrder(mainCharacters));
            TopAltCharacters.Add(ApplyInitialOrder(topAltCharacters));

            var allAltCharacters = new List<CharacterViewModel>();
            foreach (var c in _initialCharacters)
            {
                allAltCharacters.AddRange(c.Alts.Where(FilterCharacter).Select(a => new CharacterViewModel(a, c.Main.Name)));
            }

            AllAltCharacters.Add(ApplyInitialOrder(allAltCharacters));
        }

        public bool CanFilter
        {
            get { return _initialCharacters != null && _initialCharacters.Any(); }   
        }

        private void ClearAll()
        {
            MainCharacters.SelectedCharacter = null;
            MainCharacters.Clear();
            AltCharacters.Clear();
            TopAltCharacters.Clear();
            AllAltCharacters.Clear();
        }

        private static IEnumerable<CharacterViewModel> ApplyInitialOrder(IEnumerable<CharacterViewModel> characters)
        {
            return characters.OrderByDescending(c => c.AverageItemLevel);
        }

        private bool FilterCharacter(GuildInfoCharacter character)
        {
            var result =
                (MinItemLevel == 0 || character.AverageItemLevel >= MinItemLevel) &&
                (MinLevel == 0 || character.Level >= MinLevel) &&
                Classes.Where(c => c.IsSelected).Select(c => c.Element.Id).Contains(character.Class) &&
                !_calculatedRanksToExclude.Contains(character.GuildRank);

            return result;
        }

        private void CalculateRanksToExclude()
        {
            _calculatedRanksToExclude = new List<int>();
            if (string.IsNullOrWhiteSpace(RanksToExclude)) return;
            var split = RanksToExclude.Split(',');
            foreach (var s in split)
            {
                int rankToExclude;
                int.TryParse(s, out rankToExclude);
                if (rankToExclude != 0)
                {
                    _calculatedRanksToExclude.Add(rankToExclude);
                }
            }
        }

        private string GetConfigValue(string key)
        {
            return _config.AppSettings.Settings[key].Value;
        }

        private void SetConfigValue(string key, string value)
        {
            _config.AppSettings.Settings[key].Value = value;
            _config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
