using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace Simulation
{
    /// <summary>
    /// Classe responsável por controlar os gatilhos de tempo
    /// para os parâmetros de simulação especificados
    /// </summary>
    public class Simulator : INotifyPropertyChanged
    {
        bool _isRunning;
        bool _isPaused;
        bool _isStoped = true;
        SimulationType simulationType;
        TimeSpan simulationTime = TimeSpan.Zero;
        TimeSpan simulationDuration = TimeSpan.FromSeconds(2);
        TimeSpan simulationInterval = TimeSpan.FromMilliseconds(50);
        DispatcherTimer timer;
        CancellationTokenSource _tokenSource;

        /// <summary>
        /// Determina se a simulação está em andamento
        /// </summary>
        public bool IsRunning 
        { 
            get => _isRunning;
            private set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        /// <summary>
        /// Determina se a simulação está em pausa
        /// </summary>
        public bool IsPaused 
        { 
            get => _isPaused;
            private set
            {
                _isPaused = value;
                OnPropertyChanged(nameof(IsPaused));
            }
        }

        /// <summary>
        /// Determina se a simulação está parada
        /// </summary>
        public bool IsStoped 
        { 
            get => _isStoped;
            set
            {
                _isStoped = value;
                OnPropertyChanged(nameof(IsStoped));
            }
        }

        /// <summary>
        /// Determina o tipo de simulação a ser executada
        /// </summary>
        public SimulationType SimulationType 
        { 
            get => simulationType;
            set
            {
                simulationType = value;
                OnPropertyChanged(nameof(SimulationType));
            }
        }

        /// <summary>
        /// Determina a duração da simulação transiente
        /// </summary>
        public TimeSpan SimulationDuration 
        { 
            get => simulationDuration;
            set
            {
                simulationDuration = value;
                OnPropertyChanged(nameof(SimulationDuration));
            }
        }

        /// <summary>
        /// Determina o intervalo entre os eventos de tempo
        /// </summary>
        public TimeSpan SimulationInterval 
        { 
            get => simulationInterval;
            set
            {
                simulationInterval = value;
                OnPropertyChanged(nameof(SimulationInterval));
            }
        }

        /// <summary>
        /// Obtém o instante atual da simulação
        /// </summary>
        public TimeSpan SimulationTime 
        { 
            get => simulationTime;
            private set
            {
                simulationTime = value;
                OnPropertyChanged(nameof(SimulationTime));
            }
        }

        /// <summary>
        /// Inicia a simulação
        /// </summary>
        public void Start()
        {
            IsRunning = true;
            IsStoped = false;
            IsPaused = false;

            if (!IsPaused)
            {
                _tokenSource = new CancellationTokenSource();
            }

            Simule(_tokenSource.Token);
        }


        /// <summary>
        /// Pausa a simulação
        /// </summary>
        public void Pause()
        {
            IsRunning = false;
            IsStoped = true;
            IsPaused = true;

            _tokenSource.Cancel();
        }


        /// <summary>
        /// Para a simulação e zera o tempo de simulação
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
            IsStoped = true;
            IsPaused = false;

            _tokenSource.Cancel();
            SimulationTime = TimeSpan.Zero;
        }

        private void Simule(CancellationToken token)
        {            
            switch (SimulationType)
            {
                //lógica da simulação de tempo real
                case SimulationType.RealTime:
                    timer = new DispatcherTimer(DispatcherPriority.MaxValue); //cria um novo timer
                    timer.Interval = simulationInterval; //atribui o intervalo de tempo ao timer
                    timer.Tick += (s, a) => //especifica uma expressão lambda para o evento tick
                    {
                        if (token.IsCancellationRequested) //se o cancelamento for requisitado pare o timer e retorne
                        {
                            timer.Stop();
                            return;
                        }
                        SimulationTime += simulationInterval; //contabiliza o tempo atual da simulação

                        Tick?.Invoke(this, new SimulationTickEventArgs() { CurrentTime = SimulationTime }); //dispara o evento tick
                    };
                    timer.Start();
                    break;

                //lógica da simulação transiente
                case SimulationType.Transient:

                    if (simulationInterval > simulationDuration) //verifica se os parâmetros são válidos
                        throw new ArgumentException("O intervalo de simulação deve ser menor que a duração da simulação.");

                    while(SimulationTime <= simulationDuration) //execute a simulação no intervalo de tempo até a duração
                    {
                        if (token.IsCancellationRequested) //se o cancelamento for requisitado pare o laço while
                            break;

                        SimulationTime += simulationInterval; //contabiliza o tempo atual da simulação

                        Tick?.Invoke(this, new SimulationTickEventArgs() { CurrentTime = SimulationTime });//dispara o evento tick
                    }
                    Stop();
                    break;
            }
        }


        private void OnPropertyChanged(string propertyName) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Notifica uma alteração de proriedade da classe
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifica um disparo de tempo
        /// </summary>
        public event EventHandler<SimulationTickEventArgs> Tick;
    }
}
