using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chat_Program.Frontend.Custom_Controls
{
	/// <summary>
	/// Interaction logic for ProfileButton.xaml
	/// </summary>
	public partial class ProfileButton : UserControl
    {
        public ProfileButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public SolidColorBrush StrokeBrush
        {
            get { return (SolidColorBrush)GetValue(StrokeBrushProperty); }
            set { SetValue(StrokeBrushProperty, value); }
        }

        public bool IsOnline
        {
            get { return (bool)GetValue(IsOnlineProperty); }
            set { SetValue(IsOnlineProperty, value); }
        }

        public ImageSource ProfileImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Dependency Property enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOnlineProperty = DependencyProperty.Register(nameof(IsOnline), typeof(bool), typeof(ProfileButton));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ProfileImageSource), typeof(ImageSource), typeof(ProfileButton));
        public static readonly DependencyProperty StrokeBrushProperty = DependencyProperty.Register(nameof(StrokeBrush), typeof(SolidColorBrush), typeof(ProfileButton));
    }
}
