using DictionaryImporter.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DictionaryImporter.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        // Registry path to save test tool settings
        private string RegPath = "HKEY_CURRENT_USER\\SOFTWARE\\DictionaryImporter";
        // Word difficulty levels.
        private ObservableCollection<Tuple<string, int>> _difficultyLevels = null;
        public ObservableCollection<Tuple<string, int>> DifficultyLevels
        {
            get { return _difficultyLevels; }
            set { SetProperty(ref _difficultyLevels, value); }
        }
        // Selected Word difficulty level.
        private int _selectedDifficulty = -1;
        public int SelectedDifficulty
        {
            get { return _selectedDifficulty; }
            set { SetProperty(ref _selectedDifficulty, value); }
        }
        // Max word length.
        private string _maxWordSize = Defines.MAX_WORD_SIZE.ToString();
        public string MaxWordSize
        {
            get { return _maxWordSize; }
            set { SetProperty(ref _maxWordSize, value); }
        }
        // Button command handlers.
        public ButtonCommand AddSelectedWordsCommand { get; set; }
        public ButtonCommand LoadDictionaryCommand { get; set; }
        // Words listbox
        private ObservableCollection<string> _wordsList = null;
        public ObservableCollection<string> WordsList
        {
            get { return _wordsList; }
            set { SetProperty(ref _wordsList, value); }
        }

        public MainPageViewModel()
        {
            Initialize();
        }
                
        private void Initialize()
        {
            AddSelectedWordsCommand = new ButtonCommand(AddSelectedWordsClicked);
            LoadDictionaryCommand = new ButtonCommand(LoadDictionaryClicked);
            WordsList = new ObservableCollection<string>();
            DifficultyLevels = new ObservableCollection<Tuple<string, int>>();
            DifficultyLevels.Add(new Tuple<string, int>("Easy", 0));
            DifficultyLevels.Add(new Tuple<string, int>("Medium", 1));
            DifficultyLevels.Add(new Tuple<string, int>("Hard", 2));
            SelectedDifficulty = 0;
        }

        // Load text file
        private void LoadDictionaryClicked(object sender, EventArgs e)
        {
            var path = "";
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            var regValue = Registry.GetValue(RegPath, "WordsListFile", null);
            if (regValue != null)
                path = regValue.ToString();
            if (!string.IsNullOrEmpty(path))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(path);
            }
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                path = dlg.FileName;
                Registry.SetValue(RegPath, "WordsListFile", path);
                LoadDictionaryText(path);
            }
        }

        // Load text file
        private void LoadDictionaryText(string path)
        {
            var words = File.ReadAllLines(path);
            var filtered = from item in words
                           where item.Length >= Defines.MIN_WORD_SIZE && item.Length <= Convert.ToInt32(MaxWordSize) && !item.Contains("'s")
                           select item;
            WordsList = new ObservableCollection<string>(filtered);
        }

        // Save selected words in database
        private void AddSelectedWordsClicked(object sender, EventArgs e)
        {

        }

    }
}
