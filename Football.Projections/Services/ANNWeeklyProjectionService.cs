using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Microsoft.Extensions.Options;

namespace Football.Projections.Services
{
    public class ANNWeeklyProjectionService(IArtificialNeuralNetworkService<QBModelWeek> annService, IProjectionModelCalculator projectionModelCalculator, IPlayersService playersService, IMatrixCalculator matrixCalculator,
        IOptionsMonitor<WeeklyTunings> weeklyTunings, IOptionsMonitor<Season> season) : IANNWeeklyProjectionService
    {
        private readonly WeeklyTunings _weeklyTunings = weeklyTunings.CurrentValue;
        private readonly Season _season = season.CurrentValue;
        public async Task<List<double>> CalculateProjections(Position position)
        {
            var trainingData = matrixCalculator.NeuralNetworkMatrix(await projectionModelCalculator.QBWeeklyProjectionModel());
            var results = matrixCalculator.DependentVector(await projectionModelCalculator.WeeklyFantasyProjectionModel(position), Model.FantasyPoints);
            var player = await playersService.GetPlayer(608);
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var weightedVector = await projectionModelCalculator.GetWeeklyWeightedAverageVector(player, currentWeek, _weeklyTunings, neuralNetwork: true);

            await annService.TrainArtificialNeuralNetwork(trainingData, results, 1, .01);
            var temp = (await annService.CalculateForwardPass(weightedVector));
            return [.. temp];

        }
    }
}
