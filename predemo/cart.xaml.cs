using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using ClassLibrary;
using Microsoft.Win32;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace predemo
{
    /// <summary>
    /// Логика взаимодействия для cart.xaml
    /// </summary>
    public partial class cart : Window
    {
        public cart()
        {
            InitializeComponent();

            list.ItemsSource = values.cartItems;
            list.SelectedValuePath = "book.bookId";

            pickupPointCB.ItemsSource = DBHelper.e.pickupPoints.ToList();
            pickupPointCB.DisplayMemberPath = "address";
            pickupPointCB.SelectedValuePath = "pointId";
            pickupPointCB.SelectedIndex = 0;
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Image).Uid);
            books book = DBHelper.e.books.FirstOrDefault(x => x.bookId == id);

            string fileName = book.image;
            string path = Environment.CurrentDirectory.Replace("bin\\Debug", fileName);
            (sender as Image).Source = new BitmapImage(new Uri(path));
        }

        private void countTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            int id = Convert.ToInt32((sender as TextBox).Uid);
            cartItems ci = values.cartItems.FirstOrDefault(x=>x.book.bookId == id);

            if ((sender as TextBox).Text.Length == 0)
            {
                (sender as TextBox).Text = "1";
            }
            try
            {
                if (Convert.ToInt32((sender as TextBox).Text) == 0)
                {
                    var res = MessageBox.Show("Действительно хотите удалить книгу из корзины", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        values.cartItems.Remove(ci);
                        List<cartItems> temp = new List<cartItems>();
                        foreach (var item in values.cartItems)
                        {
                            temp.Add(item);
                        }
                        values.cartItems = temp;
                        list.ItemsSource = values.cartItems;
                        list.SelectedValuePath = "book.bookId";
                        if (values.cartItems.Count == 0)
                        {
                            this.Close();
                        }
                    }
                    else
                    {
                        (sender as TextBox).Text = "1";
                        MessageBox.Show("Действие отменено", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    ci.count = Convert.ToInt32((sender as TextBox).Text);
                }
            }
            catch
            {
                
            }
        }

        private void deleteBTN_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(list.SelectedValue);
            cartItems ci = values.cartItems.FirstOrDefault(x => x.book.bookId == id); values.cartItems.Remove(ci);
            List<cartItems> temp = new List<cartItems>();
            foreach (var item in values.cartItems)
            {
                temp.Add(item);
            }
            values.cartItems = temp;
            list.ItemsSource = values.cartItems;
            list.SelectedValuePath = "book.bookId";
            if (values.cartItems.Count == 0)
            {
                this.Close();
            }
        }

        private void formAnOrderBTN_Click(object sender, RoutedEventArgs e)
        {
            orders order = new orders();
            order.customerFIO = "asd asd asd";
            order.orderDate = DateTime.Today;
            order.pickupPointId = Convert.ToInt32(pickupPointCB.SelectedValue);
            DBHelper.e.orders.Add(order);
            DBHelper.e.SaveChanges();
            foreach (var item in values.cartItems)
            {
                booksOrder bo = new booksOrder();
                bo.orderId = order.orderId;
                bo.bookId = item.book.bookId;
                bo.quantity = item.count;
                DBHelper.e.booksOrder.Add(bo);
                DBHelper.e.SaveChanges();
            }
            SaveFileDialog sfd = new SaveFileDialog();
            PdfDocument document = new PdfDocument();
            document.Info.Title = $"Талон для получения заказа №{order.orderId}";
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont regFont = new XFont("Comic sans ms", 20);
            int height = -355;
            gfx.DrawString($"Получатель: {order.customerFIO}", regFont, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
            height += 25;
            gfx.DrawString($"Дата заказа: {order.orderDate.ToString("dd MMM. yyyy")}", regFont, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
            height += 25;
            gfx.DrawString($"Адрес получения заказа: {order.pickupPoints.address}", regFont, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
            height += 25;
            gfx.DrawString($"Состав заказа: {order.customerFIO}", regFont, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
            height += 25;
            int i = 1;
            foreach (var item in DBHelper.e.booksOrder.Where(x=>x.orderId == order.orderId))
            {
                cartItems ci = values.cartItems.FirstOrDefault(x=>x.book.bookId == item.bookId);
                gfx.DrawString($"{i++}. {item.books.name} - {item.books.price} руб. * {ci.count} шт.", regFont, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
                height += 25;
            }
            sfd.FileName = $"Талон для получения заказа №{order.orderId}.pdf";
            document.Save(sfd.FileName);
            Process.Start(sfd.FileName);
        }

        private void pickupPointCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
