using News.Models;
using News.Services;

namespace News
{
    public partial class Program
    {
        public static async Task<int> Main()
        {
            var news = new NewsService();
            var root = await news.GetEspnNews();
            foreach(var a in root.articles)
            {
                Console.WriteLine(a.headline);
            }
            return 1;
        }
    }
}

