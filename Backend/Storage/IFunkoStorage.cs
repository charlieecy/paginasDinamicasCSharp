using Backend.Error;
using CSharpFunctionalExtensions;

namespace Backend.Storage;

public interface IFunkoStorage
{
    Task<Result<string, FunkoError>> SaveFileAsync(IFormFile file, string folder);

    Task<Result<bool, FunkoError>> DeleteFileAsync(string filename);

    bool FileExists(string filename);

    string GetFullPath(string filename);
    
    string GetRelativePath(string filename, string folder = "productos");
}