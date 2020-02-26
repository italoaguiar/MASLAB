using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    /// <summary>
    /// Representa as operações válidas para o dispositivo
    /// </summary>
    public enum InstructionType
    {
        /// <summary>
        /// Função de configuração da leitura no dispositivo
        /// </summary>
        ReadSetting = 0x1,

        /// <summary>
        /// Função de configuração do intervalo de tempo entre leituras
        /// </summary>
        DelaySetting = 0x2,

        /// <summary>
        /// Função de leitura de sensores
        /// </summary>
        SensorRead = 0x3,

        /// <summary>
        /// Função de acionamento de relés
        /// </summary>
        RelayPower = 0x4,

        /// <summary>
        /// Função de leitura de estado dos relés
        /// </summary>
        RelayStatus = 0x5
    }
}
