using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TheBlogFinalMVC.Services
{
    public interface IImageService
    {
        Task<byte[]> EncodeImageAsync(IFormFile file);
        Task<byte[]> EncodeImageAsync(string fileName); //static image inside app
        string DecodeImage(byte[] data, string type);//takes image out of db and returns usable string
        string ContentType(IFormFile file);
        int Size(IFormFile file);
    }
}
