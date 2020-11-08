using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.Storage;

namespace DataAccess.Data
{
    public static class SettingsContext
    {
        private static Settings _settings { get; set; }
        public static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        public static StorageFile storageFile;

        public static async Task CreateFile()
        {
            storageFile = await storageFolder.CreateFileAsync("settings.json", Windows.Storage.CreationCollisionOption.ReplaceExisting);
        }
        public static async Task WriteToJson()
        {
            storageFile = await storageFolder.GetFileAsync("settings.json");
            await FileIO.WriteTextAsync(storageFile, "{\"status\": [\"New\", \"Ongoing\", \"Finished\"], \"category\": [\"Development\", \"Communication\", \"Other\"], \"maxItemsCount\": 4}");
        }
        public static async void GetSettingsFromJson()
        {
            var settingsFile = await FileIO.ReadTextAsync(storageFile);
            _settings = JsonConvert.DeserializeObject<Settings>(settingsFile);
        }
        public static async Task <IEnumerable<string>> GetStatus()
        {
            var statusList = new List<string>();

            foreach(var status in _settings.status)
            {
                statusList.Add(status);
            }
            return statusList;
        }
        public static async Task<IEnumerable<string>> GetCategory()
        {
           var categoryList = new List<string>();

            foreach (var category in _settings.category)
            {
                categoryList.Add(category);
            }
            return categoryList;
        }
        public static int GetMaxItemsCount()
        {
            return _settings.maxItemsCount;
        }
    }
    public class Settings
    {
        public string[] status { get; set; }
        public int maxItemsCount { get; set; }
        public string[] category { get; set; }
    }
}
