using System;
using System.Collections.Generic;
using System.Text;

namespace MASL.Controls.DataModel
{
    /// <summary>
    /// Representa as posições das conexões de entrada e saída de um tanque
    /// </summary>
    public enum ConnectionPosition
    {
        /// <summary>
        /// Entrada superior esquerda
        /// </summary>
        TopLeft,

        /// <summary>
        /// Entrada superior direita
        /// </summary>
        TopRight,

        /// <summary>
        /// Saída inferior esquerda
        /// </summary>
        BottomLeft,

        /// <summary>
        /// Saída inferior direita
        /// </summary>
        BottomRight,
    }
}
