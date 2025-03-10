﻿using Football.Players.Models;
using System.Text.Json.Serialization;

namespace Football.Players.Models
{
    public class SleeperTrendingPlayer
    {
        [JsonPropertyName("player_id")]
        public string SleeperPlayerId { get; set; } = "";

        [JsonPropertyName("count")]
        public int Adds { get; set; }
    }

    public class TrendingPlayer
    {
        public Player Player { get; set; } = new();
        public int Adds { get; set; }
    }
}
