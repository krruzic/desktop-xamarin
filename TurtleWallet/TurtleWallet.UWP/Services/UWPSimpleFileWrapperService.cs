using System;
using Xamarin.Forms;
using TurtleWallet.Services;
using TurtleWalletUWP.Services;
using System.Threading.Tasks;
using Windows.Storage;

[assembly: Dependency(typeof(UWPSimpleFileWrapperService))]
namespace TurtleWalletUWP.Services
{
    public class UWPSimpleFileWrapperService : ISimpleFileWrapperService
    {
        public async Task<bool> FileExists(string filename)
        {
            try
            {
                IStorageItem item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(filename);
                return (item != null);
            }
            catch (Exception ex)
            {
                // Should never get here 
                return false;
            }
        }

        public async Task<string> GetAbsolutePath(string filename)
        {
            if (await FileExists(filename))
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                return file.Path;
            }
            throw new System.IO.FileNotFoundException();
        }

        public async Task<string> ReadFileAsync(string filename)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.GetFileAsync(filename);
            return await FileIO.ReadTextAsync(file);
        }

        public async Task WriteFileAsync(string filename, string contents)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            System.Diagnostics.Debug.WriteLine("YOUR STORAGE FOLDER IS " + storageFolder.Path);
            StorageFile file = await storageFolder.CreateFileAsync(filename,
                                    CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(file, contents);
        }


    }
}
