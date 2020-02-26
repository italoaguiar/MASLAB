using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation
{
    /// <summary>
    /// Especifica o conjunto de parâmetros notificados
    /// no evento de disparo do clock da simulação
    /// </summary>
    public class SimulationTickEventArgs
    {
        /// <summary>
        /// Tempo atual da simulação
        /// </summary>
        public TimeSpan CurrentTime { get; set; }
    }
}
