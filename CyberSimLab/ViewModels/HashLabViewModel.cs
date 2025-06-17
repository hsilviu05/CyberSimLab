using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace CyberSimLab.ViewModels
{
    public class HashLabViewModel : INotifyPropertyChanged
    {
        private string _inputText = string.Empty;
        private int _selectedAlgorithmIndex = 0;
        private string _hashResult = string.Empty;
        private static readonly string[] Algorithms = { "SHA-256", "SHA-1", "MD5" };

        public string InputText
        {
            get => _inputText;
            set
            {
                if (_inputText != value)
                {
                    _inputText = value;
                    OnPropertyChanged();
                    UpdateHash();
                }
            }
        }

        public int SelectedAlgorithmIndex
        {
            get => _selectedAlgorithmIndex;
            set
            {
                if (_selectedAlgorithmIndex != value)
                {
                    _selectedAlgorithmIndex = value;
                    OnPropertyChanged();
                    UpdateHash();
                }
            }
        }

        public string HashResult
        {
            get => _hashResult;
            set
            {
                if (_hashResult != value)
                {
                    _hashResult = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateHash()
        {
            if (string.IsNullOrEmpty(InputText))
            {
                HashResult = string.Empty;
                return;
            }
            string algorithm = Algorithms.Length > SelectedAlgorithmIndex && SelectedAlgorithmIndex >= 0 ? Algorithms[SelectedAlgorithmIndex] : "SHA-256";
            try
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(InputText);
                byte[] hashBytes;
                switch (algorithm)
                {
                    case "SHA-256":
                        using (var sha256 = SHA256.Create())
                            hashBytes = sha256.ComputeHash(inputBytes);
                        break;
                    case "SHA-1":
                        using (var sha1 = SHA1.Create())
                            hashBytes = sha1.ComputeHash(inputBytes);
                        break;
                    case "MD5":
                        using (var md5 = MD5.Create())
                            hashBytes = md5.ComputeHash(inputBytes);
                        break;
                    default:
                        HashResult = "Unknown algorithm";
                        return;
                }
                HashResult = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();
            }
            catch (Exception ex)
            {
                HashResult = $"Error: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
