using System.Collections.Generic; 
namespace News.Models{ 

    public class Root
    {
        public string header { get; set; }
        public Link link { get; set; }
        public List<Article> articles { get; set; }
    }

}