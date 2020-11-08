using DataAccess.Data;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewIssuePage : Page
    {
        public NewIssuePage()
        {
            this.InitializeComponent();

            LoadCustomerAsync().GetAwaiter();
            LoadCategoryAsync().GetAwaiter();
        }

        private async void SaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            await SqliteContext.CreateCustomerAsync(new Customer { Name = tbCustomerName.Text });
            tbCustomerName.Text = "";
        }

        private async void btnSaveIssue_Click(object sender, RoutedEventArgs e)
        {
            await SqliteContext.CreateIssueAsync(new Issue { CustomerId = await SqliteContext.GetCustomerIdByNames(cmbCustomer.SelectedItem.ToString()), Titel = tbTitel.Text, Description = tbDescription.Text, Category = cbCategory.SelectedItem.ToString() });
            tbTitel.Text = "";
            cbCategory.Text = "";
            tbDescription.Text = "Case Added";
        }
        private async Task LoadCustomerAsync()
        {
            cmbCustomer.ItemsSource = await SqliteContext.GetCustomerNames();
        }
        private async Task LoadCategoryAsync()
        {
            cbCategory.ItemsSource = await SettingsContext.GetCategory();
        }
    }
}
