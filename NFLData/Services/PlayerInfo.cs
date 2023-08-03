using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFLData.Models;
using NFLData.Repositories;
namespace NFLData.Services
{
    public class PlayerInfo
    {
        PlayerRepository playerRepo = new PlayerRepository();
        public List<Player> GetPlayersByName(string name)
        {
 
            return  playerRepo.GetPlayersByName(name);
        }


        public List<Player> GetPlayersByPosition(string position)
        {

            return playerRepo.GetPlayersByPosition(position);
        }

        public List<Player> GetPlayersByTeam(string team)
        {

            return playerRepo.GetPlayersByTeam(team);
        }

        public List<Player> GetPlayers()
        {
            return playerRepo.GetPlayers();

        }

        public int GetPlayerId(string name)
        {
            return playerRepo.GetPlayerId(name);
        }

    }
}
