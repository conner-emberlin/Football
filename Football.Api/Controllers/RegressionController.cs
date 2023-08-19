using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Football.Models;
using Football.Services;
using MathNet.Numerics.LinearAlgebra;
using Football.Api.Helpers;
using Football.Interfaces;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegressionController : ControllerBase
    {
        private readonly IPerformRegressionService _performRegressionService;
        private readonly IRegressionModelService _regressionModelService;
        private readonly IMatrixService _matrixService;
        private readonly IPlayerService _playerService;
        private readonly IServiceHelper _serviceHelper;
        private readonly IFantasyService _fantasyService;
        public RegressionController(IPerformRegressionService performRegressionService,IPlayerService playerService, IRegressionModelService regressionModelService,
            IMatrixService matrixService, IServiceHelper serviceHelper, IFantasyService fantasyService)
        {
            _performRegressionService = performRegressionService;
            _regressionModelService = regressionModelService;
            _playerService = playerService;
            _matrixService = matrixService;
            _serviceHelper = serviceHelper;
            _fantasyService = fantasyService;   
        }
        [HttpGet("{season}/{pos}")]
        [ProducesResponseType(typeof(Vector<double>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<Vector<double>>> GetRegressionNormal(int season, int pos)
        {       
            return Ok(await _performRegressionService.PerformRegression(season, _serviceHelper.TransformPosition(pos)));
        }

        [HttpGet("mse/{season}/{pos}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<double>> GetMSE(int season, int pos)
        {
            string position = _serviceHelper.TransformPosition(pos);
            var fantasyResults = await _regressionModelService.PopulateFantasyResults(season, position);
            var coefficients = await _performRegressionService.PerformRegression(season, position);
            var actual = _matrixService.PopulateDependentVector(fantasyResults);
            double mse;
            switch (pos)
            {
                case 1:
                    List<RegressionModelQB> models = new();
                    foreach (var fr in fantasyResults)
                    {
                        var model = await _regressionModelService.PopulateRegressionModelQB(fr.PlayerId, fr.Season);
                        models.Add(model);
                    }
                    var regressorMatrix = _matrixService.PopulateQbRegressorMatrix(models);
                    mse = Math.Round((double)_performRegressionService.CalculateMSE(actual, coefficients, regressorMatrix), 2);
                    break;
                case 2:
                    List<RegressionModelRB> modelsR = new();
                    foreach (var fr in fantasyResults)
                    {
                        var modelR = await _regressionModelService.PopulateRegressionModelRb(fr.PlayerId, fr.Season);
                        modelsR.Add(modelR);
                    }
                    var regressorMatrixR = _matrixService.PopulateRbRegressorMatrix(modelsR);
                    mse = Math.Round((double)_performRegressionService.CalculateMSE(actual, coefficients, regressorMatrixR), 2);
                    break;
                case 3:
                    List<RegressionModelPassCatchers> modelsP = new();
                    foreach (var fr in fantasyResults)
                    {
                        var modelP = await _regressionModelService.PopulateRegressionModelPassCatchers(fr.PlayerId, fr.Season);
                        modelsP.Add(modelP);
                    }
                    var regressorMatrixP = _matrixService.PopulatePassCatchersRegressorMatrix(modelsP);
                    mse = Math.Round((double)_performRegressionService.CalculateMSE(actual, coefficients, regressorMatrixP), 2);
                    break;
                default: 
                    mse = 0;
                    break;
            }  
            return mse;
        }

        [HttpGet("model-error/{playerId}")]
        [ProducesResponseType(typeof(List<int>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<double>>> GetModelErrorByPlayer(int playerId)
        {
            return await _performRegressionService.ModelErrorPerSeason(playerId, await _playerService.GetPlayerPosition(playerId));
        }
    }
}
