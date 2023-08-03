using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Football.Models;
using Football.Services;
using MathNet.Numerics.LinearAlgebra;
using Football.Api.Helpers;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegressionController : ControllerBase
    {
        [HttpGet("regression/{season}/{pos}")]
        [ProducesResponseType(typeof(Vector<double>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult<Vector<double>> GetRegressionNormal(int season, int pos)
        {
            PerformRegressionService performRegressionService = new();
            ServiceHelper serviceHelper = new();
            string position = serviceHelper.TransformPosition(pos);           
            return Ok(performRegressionService.PerformRegression(season, position));
        }

        [HttpGet("regression/mse/{season}/{pos}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult<double> GetMSE(int season, int pos)
        {
            RegressionModelService regressionModelService = new();
            PerformRegressionService performRegressionService = new();
            MatrixService matrixService = new();
            ServiceHelper serviceHelper = new();
            string position = serviceHelper.TransformPosition(pos);
            var fantasyResults = regressionModelService.PopulateFantasyResults(season, position);
            var coefficients = performRegressionService.PerformRegression(season, position);
            var actual = matrixService.PopulateDependentVector(fantasyResults);
            switch (pos)
            {
                case 1:
                    var model = matrixService.PopulateQbRegressorMatrix(fantasyResults.Select(fr => regressionModelService.PopulateRegressionModelQB(fr.PlayerId, fr.Season)).ToList());
                    return Math.Round((double)performRegressionService.CalculateMSE(actual, coefficients, model),2);
                    break;
                case 2:
                    var modelR = matrixService.PopulateRbRegressorMatrix(fantasyResults.Select(fr => regressionModelService.PopulateRegressionModelRb(fr.PlayerId, fr.Season)).ToList());
                    return Math.Round((double)performRegressionService.CalculateMSE(actual, coefficients, modelR),2);
                    break;
                case 3:
                    var modelP = matrixService.PopulatePassCatchersRegressorMatrix(fantasyResults.Select(fr => regressionModelService.PopulateRegressionModelPassCatchers(fr.PlayerId, fr.Season)).ToList());
                    return Math.Round((double)performRegressionService.CalculateMSE(actual, coefficients, modelP),2);
                    break;
                default: throw new NotImplementedException();
            }         
        }
    }
}
