using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    /// <summary>
    /// Controla dispositivos seriais do mundo real
    /// </summary>
    public class SerialDevice: IDisposable
    {
        /// <summary>
        /// Cria uma novav instância de SerialDevice
        /// </summary>
        /// <param name="portName">Nome da porta serial</param>
        public SerialDevice(string portName)
        {
            serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        private SerialPort serialPort;

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string r = serialPort.ReadLine();

                if (r.Length != 4) return;


                int c1 = r[0];
                string v = "0x" + r.Substring(1, 3);
                int val = Convert.ToInt32(v,16);

                if(val > 1024)
                {   
                    return;
                }

                switch (c1 >> 4)
                {
                    case 0x1:
                        break;
                    case 0x2:
                        break;
                    case 0x3:
                        int value = val;
                        SensorEventArgs arg = new SensorEventArgs();
                        arg.Data = value;
                        arg.Sensor = (Sensors)(c1 & 0x0F);
                        SensorDataReceived?.Invoke(this, arg);
                        break;
                    case 0x4:
                        break;
                    case 0x5:
                        break;
                    case 0x6:
                        break;
                }

            }
            catch { }
        }


        /// <summary>
        /// Abre a conexão da porta serial
        /// </summary>
        public void Open()
        {
            serialPort.Open();
            serialPort.DiscardInBuffer();
        }


        /// <summary>
        /// Fecha a conexão da porta serial
        /// </summary>
        public void Close()
        {
            serialPort.Close();
        }


        /// <summary>
        /// Configura o modo de leitura do dispositivo
        /// </summary>
        /// <param name="mode">Modo de leitura</param>
        public void ConfigureReadMode(ReadMode mode)
        {            
            byte[] package = new byte[] { 0x10, 0xFF };
            if(mode == ReadMode.Continuous)
            {
                package = new byte[] { 0x11, 0xFF };
            }
            serialPort.Write(package, 0, 2);
        }

        /// <summary>
        /// Configura o intervalo de tempo entre leituras
        /// </summary>
        /// <param name="delay">Intevalo de tempo</param>
        public void ConfigureDelay(byte delay)
        {
            byte[] package = new byte[] { (byte)(0x2F & delay), 0xFF };
            serialPort.Write(package, 0, 2);
        }

        /// <summary>
        /// Solicita ao dispositivo o envio de dados quando configurado 
        /// no modo Controlled
        /// </summary>
        /// <param name="sensor">Sensor</param>
        public void RequestSensorData(Sensors sensor)
        {
            byte[] package = new byte[] { (byte)(0x30 | (0x0F & (byte)sensor)), 0xFF };
            serialPort.Write(package, 0, 2);
        }
        
        /// <summary>
        /// Solicita ao dispositivo o estado atual de um Relé
        /// </summary>
        /// <param name="relay">Relé</param>
        public void RequestRelayState(Relays relay)
        {
            byte[] package = new byte[] { (byte)(0x5F & (byte)relay), 0xFF };
            serialPort.Write(package, 0, 2);
        }

        /// <summary>
        /// Ativa ou desativa um relé no dispositivo
        /// </summary>
        /// <param name="relay">Relé</param>
        /// <param name="state">Estado</param>
        public void SetRelayState(Relays relay, DeviceState state)
        {
            if (state == DeviceState.Off)
            {
                byte[] package = new byte[] { (byte)(0x40 | (0x0F & (byte)relay)), 0xFF };
                serialPort.Write(package, 0, 2);
            }
            else
            {
                byte[] package = new byte[] { (byte)(0x50 | (0x0F & (byte)relay)), 0xFF };
                serialPort.Write(package, 0, 2);
            }
        }

        /// <summary>
        /// Destroi a conexão da porta serial.
        /// </summary>
        public void Dispose()
        {
            serialPort.Dispose();
        }

        /// <summary>
        /// Notifica a recepção de dados por parte do dispositivo.
        /// </summary>
        public event EventHandler<SensorEventArgs> SensorDataReceived;
    }
}
