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
        public readonly IPerformRegressionService _performRegressionService;
        public readonly IRegressionModelService _regressionModelService;
        public readonly IMatrixService _matrixService;
        public RegressionController(IPerformRegressionService performRegressionService, IRegressionModelService regressionModelService,
            IMatrixService matrixService)
        {
            _performRegressionService = performRegressionService;
            _regressionModelService = regressionModelService;
            _matrixService = matrixService;
        }
        [HttpGet("{season}/{pos}")]
        [ProducesResponseType(typeof(Vector<double>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult<Vector<double>> GetRegressionNormal(int season, int pos)
        {
            ServiceHelper serviceHelper = new();
            string position = serviceHelper.TransformPosition(pos);           
            return Ok(_performRegressionService.PerformRegression(season, position));
        }

        [HttpGet("mse/{season}/{pos}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult<double> GetMSE(int season, int pos)
        {
            ServiceHelper serviceHelper = new();
            string position = serviceHelper.TransformPosition(pos);
            var fantasyResults = _regressionModelService.PopulateFantasyResults(season, position);
            var coefficients = _performRegressionService.PerformRegression(season, position);
            var actual = _matrixService.PopulateDependentVector(fantasyResults);
            switch (pos)
            {
                case 1:
                    var model = _matrixService.PopulateQbRegressorMatrix(fantasyResults.Select(fr => _regressionModelService.PopulateRegressionModelQB(fr.PlayerId, fr.Season)).ToList());
                    return Math.Round((double)_performRegressionService.CalculateMSE(actual, coefficients, model),2);
                    break;
                case 2:
                    var modelR = _matrixService.PopulateRbRegressorMatrix(fantasyResults.Select(fr => _regressionModelService.PopulateRegressionModelRb(fr.PlayerId, fr.Season)).ToList());
                    return Math.Round((double)_performRegressionService.CalculateMSE(actual, coefficients, modelR),2);
                    break;
                case 3:
                    var modelP = _matrixService.PopulatePassCatchersRegressorMatrix(fantasyResults.Select(fr => _regressionModelService.PopulateRegressionModelPassCatchers(fr.PlayerId, fr.Season)).ToList());
                    return Math.Round((double)_performRegressionService.CalculateMSE(actual, coefficients, modelP),2);
                    break;
                default: throw new NotImplementedException();
            }         
        }       
    }
}
