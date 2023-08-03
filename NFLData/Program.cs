using NFLData.Services;


namespace NFLData
{

    public partial class Program {
        public static async Task<int> Main()
        {
            Console.WriteLine("Retrieve Player Info");
            Console.WriteLine("Search by Name, Position, or Team");
            var choice = Console.ReadLine();
            var playerInfo = new PlayerInfo();

            switch (choice?.Trim().ToLower())
            {

                case "name":
                    Console.WriteLine("Enter Name to Search");
                    var name = Console.ReadLine();
                    var players = playerInfo.GetPlayersByName(name.Trim());
                    foreach (var player in players)
                    {
                        Console.WriteLine(player.Name + "- Position: " + player.Position + ", Team: " + player.Team);  
                    }
                    break;

                case "position":
                    Console.WriteLine("Enter Position to Search");
                    var position = Console.ReadLine();
                    var playersPosition = playerInfo.GetPlayersByPosition(position.Trim());
                    foreach (var player in playersPosition)
                    {
                        Console.WriteLine(player.Name + "- Position: " + player.Position + ", Team: " + player.Team);
                    }
                    break;

                case "team":
                    Console.WriteLine("Enter team to Search");
                    var team = Console.ReadLine();
                    var playersTeam = playerInfo.GetPlayersByTeam(team.Trim());
                    foreach (var player in playersTeam)
                    {
                        Console.WriteLine(player.Name + "- Position: " + player.Position + ", Team: " + player.Team);
                    }
                    break;
                default:
                    Console.WriteLine("No Choice Entered");
                    break;

            }
            return 1;

        }
    }

}