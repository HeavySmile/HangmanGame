using CrypticWizard.RandomWordGenerator;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace HangmanGame
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private const int maxMistakes = 6;

        private int mistakeCount = 0;
        private string word = "";
        private string spotlight;
        private string gameStatus = $"Mistakes: 0 of 6";
        private List<char> guessed = new List<char>();
        private List<char> letters = new List<char>();
        private List<string> images = new List<string>() 
        { 
            "img0.jpg", 
            "img1.jpg", 
            "img2.jpg", 
            "img3.jpg", 
            "img4.jpg", 
            "img5.jpg", 
            "img6.jpg" 
        };

        public string Spotlight 
        {
            get => spotlight;
            set
            {
                spotlight = value;
                OnPropertyChanged();
            }
        }
        public List<char> Letters
        {
            get => letters;
            set
            {
                letters = value;
                OnPropertyChanged();
            }
        }
        public string GameStatus 
        { 
            get => gameStatus; 
            set
            {
                gameStatus = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            InitializeComponent();
            Letters.AddRange(Enumerable.Range('a', 'z' - 'a' + 1).Select(i => (char)i));
            
            BindingContext = this;
            
            PickWord();
            CalculateWord(word, guessed);
        }
        
        private void PickWord()
        {
            WordGenerator generator = new WordGenerator();
           
            word = generator.GetWord(WordGenerator.PartOfSpeech.noun);
            Debug.WriteLine(word);
        }

        private void CalculateWord(string newWord, List<char> guessed)
        {
            word = newWord;

            var temp = newWord.Select(x => (guessed.IndexOf(x) != -1 ? x : '_')).ToArray();
            Spotlight = string.Join(' ', temp);
        }

        private void HandleGuess(char ch)
        {
            if (!guessed.Contains(ch)) guessed.Add(ch);
            
            CalculateWord(word, guessed);
            CheckForVictory();

            if (!word.Contains(ch))
            {
                mistakeCount++;
                GameStatus = $"Mistakes: {mistakeCount} of {maxMistakes}";
                CheckForLoss();
                hangmanImg.Source = images[mistakeCount];
            }
        }

        private void ChangeLettersStatus(bool status)
        {
            letterContainer.Children.Select(x => (x as Button).IsEnabled = status).ToList();
        }

        private void CheckForLoss()
        {
            if (mistakeCount == maxMistakes)
            {
                resultLbl.Text = "You lost!";
                resultLbl.IsVisible = true;
                ChangeLettersStatus(false);
            }
        }

        private void CheckForVictory()
        {
            if (Spotlight.Replace(" ", "") == word) 
            {
                resultLbl.Text = "You won!";
                resultLbl.IsVisible = true;
                ChangeLettersStatus(false);
            }
        }

        private void letterButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (button != null) 
            { 
                var ch = button.Text;
                button.IsEnabled = false;
                HandleGuess(ch[0]);
            }
        }

        private void resetBtn_Clicked(object sender, EventArgs e)
        {
            mistakeCount = 0;
            guessed.Clear();
            PickWord();
            CalculateWord(word, guessed);
            resultLbl.IsVisible = false;
            GameStatus = $"Mistakes: 0 of 6";
            hangmanImg.Source = "img0.jpg";
            ChangeLettersStatus(true);
        }
    }
}