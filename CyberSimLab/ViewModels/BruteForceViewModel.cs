using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CyberSimLab.ViewModels
{
    public class BruteForceViewModel : INotifyPropertyChanged
    {
        private string _targetPassword = string.Empty;
        private bool _showPassword = false;
        private string _allowedCharacters = "abcdefghijklmnopqrstuvwxyz";
        private int _passwordLength = 4;
        private string _currentGuess = string.Empty;
        private string _elapsedTime = "00:00:00";
        private double _progress = 0.0;
        private string _status = "Idle";
        private bool _isRunning = false;
        private CancellationTokenSource? _cts;
        private DateTime _startTime;

        public string TargetPassword
        {
            get => _targetPassword;
            set { _targetPassword = value; OnPropertyChanged(); UpdateCanStart(); }
        }
        public bool ShowPassword
        {
            get => _showPassword;
            set { _showPassword = value; OnPropertyChanged(); }
        }
        public string AllowedCharacters
        {
            get => _allowedCharacters;
            set { _allowedCharacters = value; OnPropertyChanged(); UpdateCanStart(); }
        }
        public int PasswordLength
        {
            get => _passwordLength;
            set { _passwordLength = value; OnPropertyChanged(); UpdateCanStart(); }
        }
        public string CurrentGuess
        {
            get => _currentGuess;
            set { _currentGuess = value; OnPropertyChanged(); }
        }
        public string ElapsedTime
        {
            get => _elapsedTime;
            set { _elapsedTime = value; OnPropertyChanged(); }
        }
        public double Progress
        {
            get => _progress;
            set { _progress = value; OnPropertyChanged(); }
        }
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    OnPropertyChanged();
                    (StartCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (StopCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    UpdateCanStart();
                }
            }
        }
        private bool _canStart = true;
        public bool CanStart
        {
            get => _canStart;
            set { _canStart = value; OnPropertyChanged(); (StartCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public BruteForceViewModel()
        {
            StartCommand = new RelayCommand(async _ => await StartBruteForce(), _ => CanStart);
            StopCommand = new RelayCommand(_ => StopBruteForce(), _ => IsRunning);
        }

        private void UpdateCanStart()
        {
            CanStart = !IsRunning && !string.IsNullOrEmpty(TargetPassword) && !string.IsNullOrEmpty(AllowedCharacters) && PasswordLength > 0;
        }

        private async Task StartBruteForce()
        {
            if (IsRunning) return;
            IsRunning = true;
            Status = "Cracking...";
            Progress = 0;
            CurrentGuess = string.Empty;
            _cts = new CancellationTokenSource();
            _startTime = DateTime.Now;
            var timer = Task.Run(async () =>
            {
                while (IsRunning)
                {
                    ElapsedTime = (DateTime.Now - _startTime).ToString(@"hh\:mm\:ss");
                    await Task.Delay(100);
                }
            });
            try
            {
                await BruteForceAsync(_cts.Token);
            }
            catch (OperationCanceledException) { Status = "Stopped"; }
            finally
            {
                IsRunning = false;
                _cts = null;
            }
        }

        private void StopBruteForce()
        {
            _cts?.Cancel();
            IsRunning = false;
            Status = "Stopped";
        }

        private async Task BruteForceAsync(CancellationToken token)
        {
            var charset = AllowedCharacters;
            int length = PasswordLength;
            long total = (long)Math.Pow(charset.Length, length);
            char[] guess = new char[length];
            for (long i = 0; i < total; i++)
            {
                if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();
                long n = i;
                for (int pos = length - 1; pos >= 0; pos--)
                {
                    guess[pos] = charset[(int)(n % charset.Length)];
                    n /= charset.Length;
                }
                string guessStr = new string(guess);
                CurrentGuess = guessStr;
                Progress = (double)(i + 1) / total;
                if (guessStr == TargetPassword)
                {
                    Status = $"Found: {guessStr}";
                    return;
                }
                await Task.Delay(1); // Simulate work
            }
            Status = "Not found";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // RelayCommand with RaiseCanExecuteChanged
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;
        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object? parameter) => _execute(parameter);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
