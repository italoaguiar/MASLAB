using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation
{
    /// <summary>
    /// Especifica o tipo de simulação a ser executada
    /// </summary>
    public enum SimulationType
    {
        /// <summary>
        /// Executa uma simulação instantânea para um intervalo de
        /// tempo especificado.
        /// </summary>
        Transient,

        /// <summary>
        /// Executa a simulação em tempo real até que o usuário
        /// pause ou interrompa a simulação.
        /// </summary>
        RealTime
    }
}
