﻿using News.Models;

namespace News.Interfaces {

    public interface INewsService
    {
        public Task<EspnNews> GetEspnNews(); 
    }
}