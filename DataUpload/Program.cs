using DataUpload.Uploads;

namespace DataUpload
{

    public partial class Program
    {
        private static string filepath = @"C:\\NFLData\\pro-football-reference.csv";

        public static async Task<int> Main()
        {
            Console.WriteLine("What is the data type?");
            var choice = Console.ReadLine();
            var playerUpload = new PlayerUpload();
            var statsUpload = new StatsUpload();
            

            switch (choice.Trim().ToLower())
            {
                case "player":
                    Console.WriteLine("Uploading Players...");
                   var players =  playerUpload.PlayerFileUpload(filepath);
                    var count = await playerUpload.PlayerInsert(players);
                    Console.WriteLine("Upload Complete, " + count + " players added");
                    break;

                case "passingstat":
                    Console.WriteLine("Uploading passing stats...");
                    var passingStats = statsUpload.PassingStatFileUpload(filepath);
                    var passingCount = await statsUpload.PassingStatInsert(passingStats);
                    Console.WriteLine("Upload Complete, " + passingCount + " stats added");
                       break;

                case "receivingstat":
                    Console.WriteLine("Uploading receiving stats...");
                    var receivingStats = statsUpload.ReceivingStatFileUpload(filepath);
                    var receivingCount = await statsUpload.ReceivingStatInsert(receivingStats);
                    Console.WriteLine("Upload Complete, " + receivingCount + " stats added");
                    break;


                case "rushingstat":
                    Console.WriteLine("Uploading rushing stats...");
                    var rushingStats = statsUpload.RushingStatFileUpload(filepath);
                    var rushingCount = await statsUpload.RushingStatInsert(rushingStats);
                    Console.WriteLine("Upload Complete, " + rushingCount + " stats added");
                    break;

                case "season":
                    Console.WriteLine("Uploading season stats...");
                    var seasonInfo = playerUpload.PlayerSeasonUpload(filepath);
                    var seasonCount = await playerUpload.PlayerSeasonInsert(seasonInfo);
                    Console.WriteLine("Upload Complete, " + seasonCount + " stats added");
                    break;

                default:
                    break;



            }



            return 1;
        }



    }





}