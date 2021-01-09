using System.ComponentModel;

namespace Chat_Program
{
	public class Message : INotifyPropertyChanged
	{
		private string _contents = string.Empty;
		public string Contents 
		{
			get => _contents;
			set
			{
				if (value != _contents)
				{
					_contents = value;
					OnPropertyChanged("Contents");
				}
			}
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
