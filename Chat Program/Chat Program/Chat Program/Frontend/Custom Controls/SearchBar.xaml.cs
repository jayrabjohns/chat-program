using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat_Program.Frontend.Custom_Controls
{
	/// <summary>
	/// Interaction logic for SearchBar.xaml
	/// </summary>
	public partial class SearchBar : UserControl, INotifyPropertyChanged
	{
		public Func<string, Task> OnSearch { get; set; }

		private string _placeholderText = "Enter to search...";
		private string PlaceholderText
		{
			get => _placeholderText;
			set
			{
				if (_placeholderText != value)
				{
					_placeholderText = value;
					OnPropertyChanged(nameof(PlaceholderText));
				}
			}
		}

		private string _searchQueryText = string.Empty;
		public string SearchQueryText 
		{
			get => _searchQueryText;
			set 
			{ 
				if (_searchQueryText != value)
				{
					_searchQueryText = value;
					OnPropertyChanged(nameof(SearchQueryText));
				}
			}
		}

		public SearchBar()
		{
			InitializeComponent();
			this.DataContext = this;
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		private async void AcceptQueryAsyncEntry()
		{
			if (OnSearch != null)
			{
				await OnSearch(SearchQueryText);
			}
		}

		private void searchQuery_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && e.KeyboardDevice.IsKeyDown(Key.LeftShift))
			{
				AcceptQueryAsyncEntry();
			}
		}
	}
}
