using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation
{
    /// <summary>
    /// Representa os dados retornados pelo metodo
    /// de atualização da simulação
    /// </summary>
    public class SimulationData
    {
        /// <summary>
        /// Cria uma nova instância de SimulationData
        /// </summary>
        public SimulationData()
        {

        }

        /// <summary>
        /// Cria uma nova instância de SimulationData
        /// </summary>
        /// <param name="level">Nível calculado</param>
        /// <param name="leftOutput">Vazão calculada na saída esquerda</param>
        /// <param name="rightOutput">Vazão calculada na saída direita</param>
        public SimulationData(double level, double leftOutput, double rightOutput)
        {
            Level = level;
            LeftOutput = leftOutput;
            RightOutput = rightOutput;
        }

        /// <summary>
        /// Representa o nível do tanque calculado na simulação
        /// </summary>
        public double Level { get; set; }

        /// <summary>
        /// Representa a vazão do tanque calculada da simulação
        /// </summary>
        public double LeftOutput { get; set; }

        /// <summary>
        /// Representa a vazão do tanque calculada da simulação
        /// </summary>
        public double RightOutput { get; set; }
    }
}
