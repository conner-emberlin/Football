namespace News.Models{ 

    public class Api
    {
        public News news { get; set; }
        public Self self { get; set; }
        public Leagues leagues { get; set; }
        public Teams teams { get; set; }
        public Athletes athletes { get; set; }
    }

}