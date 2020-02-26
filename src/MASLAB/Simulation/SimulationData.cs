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
        /// <param name="output">Vazão calculada</param>
        public SimulationData(double level, double output)
        {
            Level = level;
            Output = output;
        }

        /// <summary>
        /// Representa o nível do tanque calculado na simulação
        /// </summary>
        public double Level { get; set; }

        /// <summary>
        /// Representa a vazão do tanque calculada da simulação
        /// </summary>
        public double Output { get; set; }
    }
}
