using Football.Models;
using Football.Projections.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Options;
using Serilog;

namespace Football.Projections.Services
{
    public class ArtificialNeuralNetwork<T> : IArtificialNeuralNetworkService<T> where T : class
    {
        private readonly int _inputSize;
        private readonly int _hiddenSize;
        private readonly int _outputSize;
        private Matrix<double> _weights1;
        private Matrix<double> _weights2;
        private Vector<double> _biases1;
        private Vector<double> _biases2;

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
            _weights1 = Matrix<double>.Build.Random(_hiddenSize, _inputSize);
            _biases1 = Vector<double>.Build.Random(_hiddenSize);
            _weights2 = Matrix<double>.Build.Random(_outputSize, _hiddenSize);
            _biases2 = Vector<double>.Build.Random(_outputSize);
        }
        public async Task<Vector<double>> CalculateForwardPass(Vector<double> input) 
        { 
            return await Task.Run(() => _weights2 * _matrixCalculator.ReLU(_weights1 * input + _biases1) + _biases2);

        } 
        public async Task TrainArtificialNeuralNetwork(Matrix<double> inputs, Vector<double> targets, int epochs, double learningRate)
        {
            await Task.Run(() =>
            {
                for (int epoch = 0; epoch < epochs; epoch++)
                {
                    double totalLoss = 0;

                    for (int i = 0; i < inputs.RowCount; i++)
                    {
                        var input = inputs.Row(i);
                        var target = targets[i];

                        var hidden = _matrixCalculator.ReLU(_weights1 * input + _biases1);
                        var output = _weights2 * hidden + _biases2;

                        double loss = Math.Pow(output[0] - target, 2);
                        totalLoss += loss;

                        var outputError = Vector<double>.Build.DenseOfArray([2 * (output[0] - target)]);
                        var weights2Gradient = outputError.ToColumnMatrix() * hidden.ToRowMatrix();
                        var biases2Gradient = outputError;

                        var hiddenError = _weights2.TransposeThisAndMultiply(outputError).PointwiseMultiply(_matrixCalculator.ReLUDerivative(hidden));
                        var weights1Gradient = hiddenError.ToColumnMatrix() * input.ToRowMatrix();
                        var biases1Gradient = hiddenError;

                        _weights2 -= learningRate * weights2Gradient;
                        _biases2 -= learningRate * biases2Gradient;

                        _weights1 -= learningRate * weights1Gradient;
                        _biases1 -= learningRate * biases1Gradient;
                    }

                    _logger.Information("Epoch {epoch + 1}/{epochs}, Loss: {totalLoss / inputs.RowCount:F4}");
                }
            });
        }
    }
}