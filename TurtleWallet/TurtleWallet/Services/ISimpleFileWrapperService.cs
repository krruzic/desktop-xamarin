using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TurtleWallet.Services
{
    public interface ISimpleFileWrapperService
    {
        Task<bool> FileExists(string filename);

        Task<string> GetAbsolutePath(string filename);


        Task<string> ReadFileAsync(string filename);

        Task WriteFileAsync(string filename, string contents);
    }
}
