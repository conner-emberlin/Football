using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Interfaces
{
    public interface IArtificialNeuralNetworkService<T>
    {
        Task<Vector<double>> CalculateForwardPass(Vector<double> input);
        Task TrainArtificialNeuralNetwork(Matrix<double> inputs, Vector<double> targets, int epochs, double learningRate);
    }
}
