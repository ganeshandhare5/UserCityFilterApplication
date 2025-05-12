using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using UserCityFilterApplication.Model;

namespace UserCityFilterApplication.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Holds all users fetched from the API
        private ObservableCollection<User> _allUsers = new();
        private ObservableCollection<User> _filteredUsers = new();
        private ObservableCollection<string> _cities = new();
        private string _selectedCity;

        public ObservableCollection<User> FilteredUsers
        {
            get => _filteredUsers;
            set { _filteredUsers = value; OnPropertyChanged(nameof(FilteredUsers)); }
        }

        public ObservableCollection<string> Cities
        {
            get => _cities;
            set { _cities = value; OnPropertyChanged(nameof(Cities)); }
        }

        // Exposes the selected city; triggers filtering logic when changed
        public string SelectedCity
        {
            get => _selectedCity;
            set
            {
                _selectedCity = value;
                OnPropertyChanged(nameof(SelectedCity));
                FilterUsers();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            _ = LoadUsersAsync();
        }

        // Asynchronously loads user data from remote API
        private async Task LoadUsersAsync()
        {
            using HttpClient client = new();
            var response = await client.GetStringAsync("https://jsonplaceholder.typicode.com/users");

            var users = JsonSerializer.Deserialize<ObservableCollection<User>>(response);
            _allUsers = users;
            Cities = new ObservableCollection<string> { "All" };
            foreach (var city in _allUsers.Select(u => u.Addresss.City).Distinct().OrderBy(c => c))
            {
                Cities.Add(city);
            }
            SelectedCity = "All";
        }

        // Filters the users based on selected city
        private void FilterUsers()
        {
            FilteredUsers = SelectedCity == "All"
                ? new ObservableCollection<User>(_allUsers)
                : new ObservableCollection<User>(_allUsers.Where(u => u.Addresss
                .City == SelectedCity));
        }

        // Notifies the view that a property value has changed
        private void OnPropertyChanged(string prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    
    }
}
