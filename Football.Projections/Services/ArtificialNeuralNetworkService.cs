using Football.Models;
using Football.Projections.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Serilog;
using Microsoft.Extensions.Options;

namespace Football.Projections.Services
{
    public class ArtificialNeuralNetwork<T> : IArtificialNeuralNetworkService<T> where T : class
    {
        private readonly int _inputSize;
        private readonly int _hiddenSize;
        private readonly int _outputSize;
        private Matrix<double> _weights;
        private Vector<double> _biases;

        private readonly ISettingsService _settingsService;
        private readonly IMatrixCalculator _matrixCalculator;
        private readonly ILogger _logger;
        private readonly ANNConfiguration _config;
        public ArtificialNeuralNetwork(IOptionsMonitor<ANNConfiguration> config, ISettingsService settingsService, IMatrixCalculator matrixCalculator, ILogger logger)
        {
            _config = config.CurrentValue;
            _settingsService = settingsService;
            _matrixCalculator = matrixCalculator;
            _logger = logger;

            _inputSize = (_settingsService.GetPropertiesFromModel<T>()).Count;
            _hiddenSize = _config.HiddenLayerSize;
            _outputSize = _config.OutputLayerSize;

            _weights = Matrix<double>.Build.Random(_hiddenSize, _inputSize) * .01;
            _biases = Vector<double>.Build.Random(_hiddenSize, 0);
        }
        public async Task<Vector<double>> CalculateForwardPass(Vector<double> input) 
        { 
            return await Task.Run(() => _matrixCalculator.ReLU(_weights * input + _biases));

        } 
        public async Task TrainArtificialNeuralNetwork(Matrix<double> inputs, Vector<double> targets, int epochs, double learningRate)
        {

        }
    }
}