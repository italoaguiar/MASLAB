using MASLAB.Services;
using MathNet.Numerics.OdeSolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation
{
    /// <summary>
    /// Representa as funções do código individual dos tanques
    /// </summary>
    public abstract class SimulationTank
    {
        /// <summary>
        /// Área da base do tanque
        /// </summary>
        public double TankArea { get; set; } = 1;

        /// <summary>
        /// Altura do tanque
        /// </summary>
        public double Level { get; set; } = 50;


        /// <summary>
        /// Constante da válvula
        /// </summary>
        public double ValveConstant { get; set; } = 0.5;



        /// <summary>
        /// Método chamado antes da simulação iniciar.
        /// Pode ser utilizado para inicialização de variáveis
        /// </summary>
        public virtual void OnSimulationStarting() { }

        /// <summary>
        /// Método chamado a cada atualização das entradas do tanque
        /// </summary>
        /// <param name="time">Parâmetro de tempo</param>
        /// <param name="input1">Entrada 1 do tanque</param>
        /// <param name="input2">Entrada 2 do tanque</param>
        /// <returns>Saída calculada para o tanque</returns>
        public abstract SimulationData OnUpdate(TimeSpan time, TimeSpan simulationStep, double input1, double input2);

        /// <summary>
        /// Método Runge Kutta de Quarta ordem para equações diferenciais
        /// </summary>
        /// <param name="y0">Valor Inicial</param>
        /// <param name="start">Tempo Inicial</param>
        /// <param name="end">Tempo Final</param>
        /// <param name="dydx">Equação diferencial a ser calculada</param>
        /// <returns></returns>
        public double RungeKutta4(double y0, double start, double end, Func<double, double, double> dydx)
        {
            return RungeKutta.FourthOrder(y0, start,end, 10, dydx)[9];
        }

        /// <summary>
        /// Método Runge Kutta de Segunda ordem para equações diferenciais
        /// </summary>
        /// <param name="y0">Valor Inicial</param>
        /// <param name="start">Tempo Inicial</param>
        /// <param name="end">Tempo Final</param>
        /// <param name="dydx">Equação diferencial a ser calculada</param>
        /// <returns></returns>
        public double RungeKutta2(double y0, double start, double end, Func<double, double, double> dydx)
        {
            return RungeKutta.SecondOrder(y0, start, end, 10, dydx)[9];
        }

        /// <summary>
        /// Plota um ponto no gráfico para a série definida
        /// </summary>
        /// <param name="serie">Nome da série</param>
        /// <param name="time">Instante de tempo da série</param>
        /// <param name="value">Valor a ser plotado</param>
        public void Plot(string serie, TimeSpan time, double value)
        {
            ChartService.GetService().Plot(serie, time, value);
        }

        /// <summary>
        /// Insere uma nova linha no log da simulação
        /// </summary>
        /// <param name="value">valor a ser inserido</param>
        public void Log(string value)
        {
            LogService.GetService().Log(value);
        }


        /// <summary>
        /// Insere uma nova linha no log da simulação
        /// </summary>
        /// <param name="value">valor a ser inserido</param>
        public void Log(TimeSpan time, object value)
        {
            Log(time.ToString(@"mm\:ss\.fff") + "   |   " + value.ToString());
        }
    }    
}

//using Simulation;

///// <summary>
///// Classe de simulação do Tanque
///// </summary>
//public class Tank : SimulationTank
//{
//    /// <summary>
//    /// Método de atualização do estado do tanque para cada intervalo de tempo
//    /// </summary>
//    /// <param name="tempo">Instante atual de tempo</param>
//    /// <param name="simulationStep">Intervalo de tempo</param>
//    /// <param name="entrada1">Entrada 1 do tanque</param>
//    /// <param name="entrada2">Entrada 2 do tanque</param>
//    /// <returns>Saída do tanque</returns>
//    public override double OnUpdate(TimeSpan tempo, double simulationStep, double entrada1, double entrada2)
//    {
//        return RungeKutta4(InitialTankLevel, tempo.TotalSeconds, ActualTankLevel, simulationStep, 
//            (x, t) =>
//            {
//                double qs = ValveConstant * Math.Sqrt(x);
//                double xd = (entrada1 + entrada2 - qs) / TankArea;

//                if (x < 0)
//                    return 0;

//                return xd;
//            }
//        );
//    }
//}
