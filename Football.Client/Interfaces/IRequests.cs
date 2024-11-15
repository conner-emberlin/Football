namespace Football.Client.Interfaces
{
    public interface IRequests
    {
        Task<T?> Get<T>(string path);
        Task<T?> Delete<T>(string path);
        Task<T?> Put<T>(string path);
        Task<T?> PutWithBody<T, T1>(string path, T1 body);
        Task<T?> PostWithoutBody<T>(string path);
        Task<T?> Post<T, T1>(string path, T1 model);
    }
}
