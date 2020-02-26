using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    public class SerialDevice: IDisposable
    {
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

        public void Open()
        {
            serialPort.Open();
            serialPort.DiscardInBuffer();
        }

        public void Close()
        {
            serialPort.Close();
        }

        public void ConfigureReadMode(ReadMode mode)
        {            
            byte[] package = new byte[] { 0x10, 0xFF };
            if(mode == ReadMode.Continuous)
            {
                package = new byte[] { 0x11, 0xFF };
            }
            serialPort.Write(package, 0, 2);
        }

        public void ConfigureDelay(byte delay)
        {
            byte[] package = new byte[] { (byte)(0x2F & delay), 0xFF };
            serialPort.Write(package, 0, 2);
        }

        public void RequestSensorData(Sensors sensor)
        {
            byte[] package = new byte[] { (byte)(0x30 | (0x0F & (byte)sensor)), 0xFF };
            serialPort.Write(package, 0, 2);
        }
        
        public void RequestRelayState(Relays relay)
        {
            byte[] package = new byte[] { (byte)(0x5F & (byte)relay), 0xFF };
            serialPort.Write(package, 0, 2);
        }

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

        public void Dispose()
        {
            serialPort.Dispose();
        }

        public event EventHandler<SensorEventArgs> SensorDataReceived;
    }
}
