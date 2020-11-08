using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DataAccess.Models;
using DataAccess.Data;
using Microsoft.Data.Sqlite;
using System.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IssueHistoryPage : Page
    {
        private IEnumerable<Issue> issues { get; set; }

        //public IssueViewModel ViewModel { get; set; }

        public IssueHistoryPage()
        {
            this.InitializeComponent();
            LoadIssuesAsync().GetAwaiter();
            LoadStatusAsync().GetAwaiter();
            //ViewModel = new IssueViewModel();
        }
        public async Task LoadIssuesAsync()
        {
            issues = await SqliteContext.GetIssues();
            LoadActiveIssuesAsync();
            LoadClosedIssuesAsync();
        }
        private void LoadActiveIssuesAsync()
        {
            lvActive.ItemsSource = issues
                .Where(i => i.Status != "New")
                .OrderByDescending(i => i.Created)
                .Take(SettingsContext.GetMaxItemsCount())
                .ToList();
        }
        private void LoadClosedIssuesAsync()
        {
            lvClosed.ItemsSource = issues
                .Where(i => i.Status == "Finished")
                .OrderByDescending(i => i.Created)
                .Take(SettingsContext.GetMaxItemsCount())
                .ToList();
        }
        private async Task LoadStatusAsync()
        {
            cbChangeStatus.ItemsSource = await SettingsContext.GetStatus();
        }
        private async void btnUpdateIssue_Click(object sender, RoutedEventArgs e)
        {
            await SqliteContext.UpdateIssue(new Issue { Id = ((Issue)lvActive.SelectedItem).Id, Status = cbChangeStatus.SelectedItem.ToString() });
            await LoadIssuesAsync();
            
        }
    }


}
