using System.Windows.Controls;
using LuckyShopClientTestG.ViewModel;

namespace LuckyShopClientTestG.View
{
    /// <summary>
    /// Interaction logic for ContentView.xaml
    /// </summary>
    public partial class ContentView : UserControl
    {
        public ContentView()
        {
            InitializeComponent();
            ContentViewModel contentViewModel = new ContentViewModel(); // Explicit object creation to fix CS8370
            DataContext = contentViewModel;
        }

    }
}
