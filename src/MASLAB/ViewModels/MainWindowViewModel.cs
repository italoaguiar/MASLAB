using Avalonia;
using MASL.Controls.DataModel;
using MASLAB.Models;
using Simulation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MASLAB.Services;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using OxyPlot.Series;
using MathNet.Numerics.Data.Matlab;
using MathNet.Numerics.Data.Text;
using System.Globalization;

namespace MASLAB.ViewModels
{
    /// <summary>
    /// ViewModel responsável por controlar a MainWindow
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Cria uma nova instância de MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            // atribui os eventos do helper de conexões entre os tanques
            ConnectionHelper.ConnectionStarted += OnConnectionStarted;
            ConnectionHelper.ConnectionCompleted += OnConnectionCompleted;

            Project = new Project();
            Project.Levels = new ObservableCollection<Level>();
            var level = new Level(Project);
            level.Items = new ObservableCollection<Tank>() { new Tank(level) };
            level.Name = "Nível 1";
            level.AddCommand = new CommandAdapter()
            {
                Action = (p) => { Project.Levels[0].Items.Add(new Tank(level)); }
            };
            Project.Levels.Add(level);

            Project.Connections = new ObservableCollection<Link>();

            //inicializa e configura o simulador
            Simulator = new Simulator();
            Simulator.SimulationType = SimulationType.RealTime;
            Simulator.SimulationDuration = TimeSpan.FromMinutes(5);
            Simulator.Tick += OnSimulationTick;
            Simulator.PropertyChanged += (s, a) => UpdateCommands();

            //configura o comando de abertura de arquivo
            OpenCommand = new CommandAdapter(true)
            {
                Action = (p) => Open()
            };

            //configura o comando de salvamento do projeto
            SaveCommand = new CommandAdapter(true)
            {
                Action = (p) => Save()
            };

            //configura o comando de partida do simulador
            StartCommand = new CommandAdapter(true)
            {
                Action = async (p) =>
                {
                    //limpa os dados da plotagem atual
                    if (!Simulator.IsPaused)
                    {
                        try
                        {
                            ChartService.GetService().Clear();
                            LogService.GetService().Clear();
                            ErrorService.GetService().Clear();

                            //percorre todos os tanques                    
                            foreach (var l in Project.Levels)
                                foreach (var t in l.Items)
                                {
                                    //compila o código de todos os tanques
                                    await t.Compile();

                                    //chama o método de inicialização de cada tanque
                                    t.SimulationTank.OnSimulationStarting();
                                }
                        }
                        catch (CompilationException ce)
                        {
                            ErrorService.GetService().AddRange(ce.Errors);

                            Simulator.Stop();

                            ShowErrorDialog("A simulação falhou ao iniciar. Por favor verifique a lista de erros");

                            SelectedTabIndex = 3;

                            return;
                        }
                        catch (Exception e)
                        {
                            Simulator.Stop();

                            ShowErrorDialog("Ocorreu um erro desconhecido ao iniciar a simulação:\n" + e.Message);

                            return;
                        }
                    }

                    //inicia a simulação
                    Simulator.Start();
                }
            };

            //configura o comando de pausa do simulador
            PauseCommand = new CommandAdapter(false)
            {
                Action = (p) => Simulator.Pause()
            };

            //configura o comando de parada do simulador
            StopCommand = new CommandAdapter(false)
            {
                Action = (p) => Simulator.Stop()
            };

            //configura o comando de abertura da janela de pripriedades dos tanques
            ShowTankPropertiesCommand = new CommandAdapter(true)
            {
                Action = (p) => new Views.TankProperties().SetTank((Tank)p).ShowDialog(App.MainWindow)
            };

            //configura o comando de remoção de conexões entre tanques
            RemovePipeCommand = new CommandAdapter(true)
            {
                Action = (p) => Project.Connections.Remove((Link)p)
            };

            //configura o comando de adição de níveis
            AddLevelCommand = new CommandAdapter(true)
            {
                Action = (p) => {
                    var level = new Level(Project);
                    level.Name = $"Nível {Project.Levels.Count + 1}";
                    level.Items = new ObservableCollection<Tank>() { new Tank(level) };
                    level.AddCommand = new CommandAdapter(true)
                    {
                        Action = (p) => level.Items.Add(new Tank(level))
                    };
                    Project.Levels.Add(level);
                }
            };

            //configura o comando para exportar imagens do diagrama
            ExportCommand = new CommandAdapter(true){ Action = Export };


            SimulationSettingsCommand = new CommandAdapter(true)
            {
                Action = async (p) =>
                {
                    Views.SimulationSettings s = new Views.SimulationSettings();
                    s.ViewModel.IsRealTime = Simulator.SimulationType == SimulationType.RealTime;

                    await s.ShowDialog(App.MainWindow);

                    Simulator.SimulationType = s.ViewModel.IsRealTime ? SimulationType.RealTime : SimulationType.Transient;
                    Simulator.SimulationDuration = s.ViewModel.Duration;
                    Simulator.SimulationInterval = s.ViewModel.Interval;
                }
            };


            RefreshChartCommand = new CommandAdapter(true)
            {
                Action = (p) => ((OxyPlot.Avalonia.PlotView)p).ResetAllAxes()
            };

            
            ChartZoomInCommand = new CommandAdapter(true)
            {
                Action = (p) => ((OxyPlot.Avalonia.PlotView)p).ZoomAllAxes(1.1)
            };

            ChartZoomOutCommand = new CommandAdapter(true)
            {
                Action = (p) => ((OxyPlot.Avalonia.PlotView)p).ZoomAllAxes(0.9)
            };

            AboutCommand = new CommandAdapter(true)
            {
                Action = (p) => IsAboutOpen = true
            };

            CheckCommandLine();

        }

        //verifica se algum arquivo foi passado como parâmetro do software
        private void CheckCommandLine()
        {
            //seleciona o primeiro arquivo encontrado com a extensão .masl
            var r = Environment.GetCommandLineArgs().FirstOrDefault(x => x.ToLower().EndsWith(".masl"));
            if(r != null)
            {
                OpenFile(r);
            }
        }

        private async void ShowErrorDialog(string message)
        {
            await MASLAB.Views.MessageBox.Show(message, "Erro");
        }

        bool linkEnabled;
        IList<Point> pointCollection = new List<Point>();
        Project project;
        bool about;
        int selectedTabIndex = 0;

        public string Greeting => "Welcome to Avalonia!";


        /// <summary>
        /// Obtém ou define a estrutura do projeto
        /// </summary>
        public Project Project
        {
            get => project;
            set => this.RaiseAndSetIfChanged(ref project, value);
        }


        /// <summary>
        /// Obtem a instância do simulador de tempo
        /// </summary>
        public Simulator Simulator { get; private set; }


        /// <summary>
        /// Obtém o comando para abrir um arquivo de projeto
        /// </summary>
        public CommandAdapter OpenCommand { get; private set; }


        /// <summary>
        /// Obtém o comando para salvar um projeto
        /// </summary>
        public CommandAdapter SaveCommand { get; private set; }


        /// <summary>
        /// Obtém o comando de partida/início do simulador
        /// </summary>
        public CommandAdapter StartCommand { get; private set; }


        /// <summary>
        /// Obtém o comando de pausa do simulador
        /// </summary>
        public CommandAdapter PauseCommand { get; private set; }


        /// <summary>
        /// Obtém o comando de parada do simulador
        /// </summary>
        public CommandAdapter StopCommand { get; private set; }


        /// <summary>
        /// Obtém o comando de exibição da janela de propriedades dos tanques
        /// </summary>
        public CommandAdapter ShowTankPropertiesCommand { get; private set; }


        /// <summary>
        /// Obtém o comando de remoção dos canos que conectam os tanques
        /// </summary>
        public CommandAdapter RemovePipeCommand { get; private set; }


        /// <summary>
        /// Obtém o comando de adicionar novos níveis
        /// </summary>
        public CommandAdapter AddLevelCommand { get; private set; }


        /// <summary>
        /// Obtém o comando para exportar um diagrama
        /// </summary>
        public CommandAdapter ExportCommand { get; private set; }


        /// <summary>
        /// Obtém o comando para exibir a janela de configurações da simulação
        /// </summary>
        public CommandAdapter SimulationSettingsCommand { get; private set; }


        /// <summary>
        /// Obtém o comando para redefinir os eixos do gráfico
        /// </summary>
        public CommandAdapter RefreshChartCommand { get; private set; }


        /// <summary>
        /// Obtém o comando para aplicar zoom ao gráfico
        /// </summary>
        public CommandAdapter ChartZoomInCommand { get; private set; }


        /// <summary>
        /// obtém o comando para remover o zoom do gráfico
        /// </summary>
        public CommandAdapter ChartZoomOutCommand { get; private set; }

        /// <summary>
        /// Obtém o comando para abrir o sobre o software
        /// </summary>
        public CommandAdapter AboutCommand { get; private set; }


        public bool IsAboutOpen
        {
            get => about;
            set => this.RaiseAndSetIfChanged(ref about, value);
        }

        /// <summary>
        /// Determina se a adição de novas conexões está habilitado
        /// </summary>
        public bool LinkEnabled 
        { 
            get => linkEnabled; 
            private set => this.RaiseAndSetIfChanged(ref linkEnabled, value);
        }


        /// <summary>
        /// Obtém a lista de pontos da conexão atual
        /// </summary>
        public IList<Point> PointCollection 
        { 
            get => pointCollection; 
            set => pointCollection = this.RaiseAndSetIfChanged(ref pointCollection, value); 
        }

        /// <summary>
        /// Obtém ou define o índice da aba selecionada
        /// </summary>
        public int SelectedTabIndex 
        { 
            get => selectedTabIndex;
            set => this.RaiseAndSetIfChanged(ref selectedTabIndex, value);
        }

        /// <summary>
        /// Método utilizado para abrir um projeto
        /// </summary>
        public async void Open()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filters.Add(new FileDialogFilter() //configura a extensão do arquivo de projeto
                {
                    Extensions = new List<string> { ".masl" },
                    Name = "Projeto do MASLAB"
                });

                //permite selecionar um único arquivo
                ofd.AllowMultiple = false;

                //abre a janela do diálogo
                var r = await ofd.ShowAsync(App.MainWindow);

                if (r.Length == 1)
                {
                    //faz a leitura do arquivo selecionado
                    OpenFile(r[0]);
                }
            }
            catch (Exception e)
            {
                await Views.MessageBox.Show(e.Message, "Error");
            }
        }

        /// <summary>
        /// Método utilizado para abrir um arquivo de projeto
        /// </summary>
        /// <param name="file">Caminho para o arquivo</param>
        public async void OpenFile(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    //faz a leitura do arquivo selecionado
                    using (StreamReader sr = new StreamReader(file))
                    {
                        //utiliza json.net para ler o arquivo do projeto
                        Project = JsonConvert.DeserializeObject<Project>(await sr.ReadToEndAsync());
                    }
                }
            }
            catch (Exception e)
            {
                await Views.MessageBox.Show(e.Message, "Error");
            }
        }

        /// <summary>
        /// Método utilizado para salvar um projeto
        /// </summary>
        private async void Save()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filters.Add(new FileDialogFilter() //configura a extensão do arquivo de projeto
                {
                    Extensions = new List<string> { "masl" },
                    Name = "Projeto do MASLAB"
                });
                sfd.DefaultExtension = "masl";

                //abre a janela do diálogo
                var r = await sfd.ShowAsync(App.MainWindow);

                if (!string.IsNullOrEmpty(r))
                {
                    SaveProjectFile(r);
                }
            }
            catch (Exception e)
            {
                await Views.MessageBox.Show(e.Message, "Error");
            }
        }


        /// <summary>
        /// Salva um arquivo do projeto desenvolvido
        /// </summary>
        /// <param name="filename">Caminho para o arquivo</param>
        private void SaveProjectFile(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                //converte o objeto do projeto para o formato json
                string t = JsonConvert.SerializeObject(Project, new JsonSerializerSettings()
                {
                    //preserva as referencias circulares do objeto
                    PreserveReferencesHandling = PreserveReferencesHandling.All
                });
                //escreve o json no arquivo
                sw.Write(t);
            }
        }


        


        /// <summary>
        /// Método chamado a cada TIC do simulador
        /// </summary>
        /// <param name="sender">Instância do simulador</param>
        /// <param name="e">Parâmetros do simulador</param>
        private void OnSimulationTick(object sender, SimulationTickEventArgs e)
        {
            try
            {
                //obtém os tanques de todos os níveis
                foreach (var t in Project.Levels.SelectMany(x => x.Items))
                {
                    //obtém as conexões de cada tanque
                    var r = Project.Connections.Where(x => x.Target.Tank == t).ToArray();
                    switch (r.Length)
                    {
                        //o tanque tem duas conexões de entrada
                        case 2:
                            t.UpdateTank(e.CurrentTime, Simulator.SimulationInterval, r[0].Origin.Tank.Output, r[1].Origin.Tank.Output);
                            break;

                        //o tanque tem uma conexão de entrada
                        case 1:
                            t.UpdateTank(e.CurrentTime, Simulator.SimulationInterval, r[0].Origin.Tank.Output, 0);
                            break;

                        //o tanque não possui conexões de entrada
                        case 0:
                            t.UpdateTank(e.CurrentTime, Simulator.SimulationInterval, 0, 0);
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                LogService.GetService().Log("!! FALHA: " + ex.Message);

                SelectedTabIndex = 2;
            }
        }

        /// <summary>
        /// Atualiza os comandos do simulador
        /// </summary>
        private void UpdateCommands()
        {
            StartCommand.SetCanExecute(!Simulator.IsRunning);
            StopCommand.SetCanExecute(Simulator.IsRunning);
            PauseCommand.SetCanExecute(Simulator.IsRunning);
        }

        /// <summary>
        /// Trata os eventos de início de conexão entre tanques
        /// </summary>
        /// <param name="c">Dados da conexão</param>
        private void OnConnectionStarted(Connection c)
        {
            PointCollection = new List<Point>();

            PointCollection.Add(c.Position);
            PointCollection.Add(new Point(c.Position.X + 10, c.Position.Y));
            PointCollection.Add(new Point(c.Position.X + 10, c.Position.Y));
            PointCollection.Add(new Point(c.Position.X + 10, c.Position.Y));

            PointCollection = new List<Point>(pointCollection);

            LinkEnabled = true;
        }

        /// <summary>
        /// Trata a finalização de conexões entre tanques
        /// </summary>
        /// <param name="l">Dados da conexão entre dois tanques</param>
        private void OnConnectionCompleted(Link l)
        {
            if(l == null)
            {
                LinkEnabled = false;
                PointCollection = new List<Point>();
                return;
            }

            LinkEnabled = false;

            var last = PointCollection.Last();
            PointCollection[PointCollection.Count - 1] = new Point(last.X, last.Y);

            Project.Connections.Add(l);
            l.Points = new List<Point>(PointCollection);
            

            PointCollection = new List<Point>();
        }

        /// <summary>
        /// Método utilizado para exportar a aba aberta do diagrama para 
        /// um arquivo de imagem PNG
        /// </summary>
        /// <param name="control">Controle de origem</param>
        private async void Export(object control)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filters.Add(new FileDialogFilter() //configura a extensão do arquivo .png
                { 
                    Name = "Imagem PNG (*.png)", 
                    Extensions = new List<string> { ".png" } 
                });
                
                saveFileDialog1.Filters.Add(new FileDialogFilter() //configura a extensão do arquivo .mat
                {
                    Extensions = new List<string> { "mat" },
                    Name = "Dados do gráfico para matriz do Matlab"
                });
                saveFileDialog1.Filters.Add(new FileDialogFilter() //configura a extensão do arquivo .csv
                {
                    Extensions = new List<string> { "csv" },
                    Name = "Dados do grafico para arquivo CSV"
                });
                saveFileDialog1.InitialFileName = "imagem.png";
                saveFileDialog1.Title = "Exportar...";
                var r = await saveFileDialog1.ShowAsync(App.MainWindow);

                if (!string.IsNullOrEmpty(r))
                {
                    switch (Path.GetExtension(r).ToLower())
                    {
                        case ".png":
                            ExportPng(control, r);
                            break;
                        case ".mat":
                            ExportMatFile(r);
                            break;
                        case ".csv":
                            ExportCsvFile(r);
                            break;
                    }
                }
                
            }
            catch (Exception e)
            {
                await Views.MessageBox.Show(e.Message, "Error");
            }
        }

        /// <summary>
        /// Exporta um arquivo de imagem para o controle informado
        /// </summary>
        /// <param name="control">Controle a ser renderizado</param>
        /// <param name="filename">Caminho para o arquivo</param>
        private void ExportPng(object control, string filename)
        {
            var target = control as TabControl;
            var pixelSize = new PixelSize((int)target.Bounds.Width, (int)target.Bounds.Height);
            var size = new Size(target.Bounds.Width, target.Bounds.Height);
            var dpiVector = new Vector(96, 96);
            using (var renderBitmap = new RenderTargetBitmap(pixelSize, dpiVector))
            {
                target.Measure(size);
                target.Arrange(new Rect(size));
                renderBitmap.Render(target);
                renderBitmap.Save(filename);
            }
        }


        /// <summary>
        /// Salva um arquivo com os dados do gráfico no formato
        /// do matlab
        /// </summary>
        /// <param name="filename">Caminho para o arquivo</param>
        private void ExportMatFile(string filename)
        {
            if (ChartService.Model != null)
            {
                var matrices = new List<MatlabMatrix>();

                foreach (var serie in ChartService.Model.Series)
                {
                    var s = (LineSeries)serie;
                    double[,] data = new double[s.Points.Count, 2];

                    for (int i = 0; i < s.Points.Count; i++)
                    {
                        data[i, 0] = s.Points[i].X;
                        data[i, 1] = s.Points[i].Y;
                    }

                    var m = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(data);

                    matrices.Add(MatlabWriter.Pack(m, s.Title.Replace(" ", "_")));
                }

                MatlabWriter.Store(filename, matrices);
            }
        }

        /// <summary>
        /// Exporta o conjunto de dados do gráfico para o formato csv
        /// </summary>
        /// <param name="filename">Caminho para o arquivo</param>
        private void ExportCsvFile(string filename)
        {
            if (ChartService.Model != null)
            {
                var matrices = new List<MatlabMatrix>();

                var max = ChartService.Model.Series
                    .Cast<LineSeries>()
                    .Max(x => x.Points.Count);

                double[,] data = new double[max, ChartService.Model.Series.Count * 2];
                string[] labels = new string[ChartService.Model.Series.Count * 2];
                for (int i = 0; i < ChartService.Model.Series.Count * 2; i = i + 2)
                {
                    LineSeries serie = (LineSeries)ChartService.Model.Series[i/2];
                    for (int j = 0; j < serie.Points.Count; j++)
                    {
                        data[j, i] = serie.Points[j].X;
                        data[j, i + 1] = serie.Points[j].Y;
                    }
                    labels[i] = serie.Title.Replace(",", "").Replace(" ", "_").Trim() + "_X";
                    labels[i + 1] = serie.Title.Replace(",", "").Replace(" ", "_").Trim() + "_Y";
                }

                var m = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(data);
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    DelimitedWriter.Write<double>(sw, m, ",", labels, formatProvider: CultureInfo.InvariantCulture.NumberFormat);
                }
            }
        }
    }
}

