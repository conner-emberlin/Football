namespace Football.UI.Interfaces
{
    public interface IRequests
    {
        public Task<T?> Get<T>(string path);
        public Task<T?> Delete<T>(string path);
        public Task<T?> Put<T>(string path);
        public Task<T?> PostWithoutBody<T>(string path);
        public Task<T?> Post<T, T1>(string path, T1 model);
    }
}
