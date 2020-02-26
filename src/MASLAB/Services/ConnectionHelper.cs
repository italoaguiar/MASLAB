using Avalonia;
using MASL.Controls.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MASLAB.Services
{
    /// <summary>
    /// Classe auxiliar para efetuar a conexão entre tanques
    /// </summary>
    public static class ConnectionHelper
    {
        private static Connection c1,c2;

        /// <summary>
        /// Notifica a requisição de uma nova conexão ou finalização de uma existente
        /// </summary>
        /// <param name="c">Conexão atual</param>
        public static void RequestConnection(Connection c)
        {
            if (c1 == null)
            {
                c1 = c;
                OnConnectionStarted(c);
            }
            else if (c2 == null)
            {
                if (c1.ConnectionType == ConnectionType.Input && c.ConnectionType == ConnectionType.Input)
                {
                    throw new ArgumentException("Não é possível conectar duas entradas diretamente");
                }
                if (c1.ConnectionType == ConnectionType.Output && c.ConnectionType == ConnectionType.Output)
                {
                    throw new ArgumentException("Não é possível conectar duas saídas diretamente");
                }

                c2 = c;
                Link l = new Link();
                l.Origin = c1.ConnectionType == ConnectionType.Input? c2 : c1;
                l.Target = c2.ConnectionType == ConnectionType.Output ? c1 : c2;
                OnConnectionRequested(l);
                Clear();
            }
        }

        private static void Clear()
        {
            c1 = c2 = null;
        }

        /// <summary>
        /// Cancela a operação de conexão atual
        /// </summary>
        public static void Cancel()
        {
            Clear();
        }

        private static void OnConnectionRequested(Link l)
        {
            ConnectionCompleted?.Invoke(l);
        }

        private static void OnConnectionStarted(Connection c)
        {
            ConnectionStarted?.Invoke(c);
        }

        public delegate void ConnectionCompletedHandler(Link link);
        public delegate void ConnectionStartedHandler(Connection c);

        /// <summary>
        /// Evento de notificação disparado quando uma conexão
        /// entre dois pontos está concluída
        /// </summary>
        public static event ConnectionCompletedHandler ConnectionCompleted;

        /// <summary>
        /// Disparado quando uma nova conexão é iniciada
        /// </summary>
        public static event ConnectionStartedHandler ConnectionStarted;
    }
}
