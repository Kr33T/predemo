using System;
using System.Collections.Generic;
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

namespace predemo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DBHelper.e = new predemoEntities();

            list.ItemsSource = DBHelper.e.books.ToList();
            list.SelectedValuePath = "bookId";
        }

        private void addToCartBTN_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(list.SelectedValue);
            books book = DBHelper.e.books.FirstOrDefault(x=>x.bookId == id);

            if (values.cartItems.Where(x=>x.book == book).Count() != 0)
            {
                values.cartItems.FirstOrDefault(x => x.book.bookId == id).count++;
            }
            else
            {
                cartItems ci = new cartItems();
                ci.book = book;
                ci.count = 1;
                values.cartItems.Add(ci);
            }

            if (values.cartItems.Count == 0)
            {
                showCartBTN.Visibility = Visibility.Hidden;
            }
            else
            {
                showCartBTN.Visibility = Visibility.Visible;
            }
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Image).Uid);
            books book = DBHelper.e.books.FirstOrDefault(x => x.bookId == id);

            string fileName = book.image;
            string path = Environment.CurrentDirectory.Replace("bin\\Debug", fileName);
            (sender as Image).Source = new BitmapImage(new Uri(path));
        }

        private void showCartBTN_Click(object sender, RoutedEventArgs e)
        {
            cart window = new cart();
            window.Show();

            window.Closing += (obj, args) =>
            {
                if (values.cartItems.Count == 0)
                {
                    showCartBTN.Visibility = Visibility.Hidden;
                }
                else
                {
                    showCartBTN.Visibility = Visibility.Visible;
                }
            };
        }
    }
}
