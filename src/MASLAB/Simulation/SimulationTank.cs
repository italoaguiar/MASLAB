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
        /// Area da base do tanque
        /// </summary>
        public double TankArea { get; set; } = 1;

        /// <summary>
        /// Altura do tanque
        /// </summary>
        public double Level { get; set; } = 50;


        /// <summary>
        /// Constante da válvula 1
        /// </summary>
        public double ValveConstant1 { get; set; } = 0.5;

        /// <summary>
        /// Constante da válvula 2
        /// </summary>
        public double ValveConstant2 { get; set; } = 0.5;

        /// <summary>
        /// Simulador
        /// </summary>
        internal Simulator Simulator { get; set; }

        /// <summary>
        /// Comando para salvar o gráfico
        /// </summary>
        internal Action<string> SaveChartCommand { get; set; }


        /// <summary>
        /// Método chamado antes da simulação iniciar.
        /// Pode ser utilizado para inicialização de variáveis
        /// </summary>
        public virtual void OnSimulationStarting() { }

        /// <summary>
        /// Método chamado quando a simulação é finalizada.
        /// </summary>
        public virtual void OnSimulationFinished() { }

        /// <summary>
        /// Método chamado a cada atualização das entradas do tanque
        /// </summary>
        /// <param name="time">Parâmetro de tempo</param>
        /// <param name="input1">Entrada 1 do tanque</param>
        /// <param name="input2">Entrada 2 do tanque</param>
        /// <param name="output1">Saída 1 do tanque</param>
        /// <param name="output2">Saída 2 do tanque</param>
        /// <param name="simulationStep">Intervalo de tempo entre chamadas de método</param>
        /// <returns>Saída calculada para o tanque</returns>
        public abstract SimulationData OnUpdate(double time, double simulationStep, double input1, double input2, double output1, double output2);

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
        /// Plota um ponto no gráfico para a série definida
        /// </summary>
        /// <param name="serie">Nome da série</param>
        /// <param name="time">Instante de tempo da série</param>
        /// <param name="value">Valor a ser plotado</param>
        public void Plot(string serie, double time, double value)
        {
            ChartService.GetService().Plot(serie, TimeSpan.FromSeconds(time), value);
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
        /// <param name="time">Instante de tempo</param>
        /// <param name="value">valor a ser inserido</param>
        public void Log(TimeSpan time, object value)
        {
            Log(time.ToString(@"mm\:ss\.fff") + "   |   " + value.ToString());
        }

        /// <summary>
        /// Insere uma nova linha no log da simulação
        /// </summary>
        /// <param name="time">Instante de tempo</param>
        /// <param name="value">valor a ser inserido</param>
        public void Log(double time, object value)
        {
            Log(TimeSpan.FromSeconds(time), value); 
        }

        /// <summary>
        /// Reinicia a contagem de tempo da simulação
        /// </summary>
        public void RestartTimer()
        {
            if (Simulator != null)
                Simulator.Reset();
        }

        /// <summary>
        /// Inicia a simulação
        /// </summary>
        public void Start()
        {
            if (Simulator != null)
                Simulator.Start();
        }

        /// <summary>
        /// Para a simulação
        /// </summary>
        public void Stop()
        {
            if (Simulator != null)
                Simulator.Stop();
        }

        /// <summary>
        /// Salva a imagem do gráfico no formato png
        /// </summary>
        /// <param name="path">Caminho para o arquivo de salvamento</param>
        public void SaveChart(string path)
        {
            SaveChartCommand?.Invoke(path);
        }

        private static Dictionary<string, object> ValueBag { get; set; }

        /// <summary>
        /// Salva um valor compartilhado na simulação
        /// </summary>
        /// <typeparam name="T">Tipo do valor a ser salvo</typeparam>
        /// <param name="key">Identificador ou nome</param>
        /// <param name="value">Valor a ser salvo</param>
        /// <returns>Retorna o valor inserido</returns>
        public T SetValue<T>(string key, T value)
        {
            if(ValueBag == null)
                ValueBag = new Dictionary<string, object>();

            if(ValueBag.ContainsKey(key))
                ValueBag[key] = value;
            else
                ValueBag.Add(key, value);

            return value;
        }

        /// <summary>
        /// Retorna um valor previamente salvo. Se o valor não for encontrado, retorna o valor padrão informado
        /// </summary>
        /// <typeparam name="T">Tipo do valor salvo</typeparam>
        /// <param name="key">Identificador ou nome</param>
        /// <param name="defaultValue">Valor padrão</param>
        /// <returns>Retorna o valor salvo previamente</returns>
        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            if (ValueBag == null)
                ValueBag = new Dictionary<string, object>();

            if(ValueBag.ContainsKey(key))
                return (T)ValueBag[key];
            else
                return defaultValue;
        }

        /// <summary>
        /// Limpa os dados da plotagem
        /// </summary>
        public void ClearChart()
        {
            ChartService.GetService().Clear();
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
