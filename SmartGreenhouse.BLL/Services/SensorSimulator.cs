using System;
using System.Timers;
using SmartGreenhouse.Models.Entities;
using SmartGreenhouse.DAL.Repositories;
using SmartGreenhouse.BLL.Services;

namespace SmartGreenhouse.BLL.Simulation
{
    /// <summary>
    /// Delegate pentru evenimentul de citire nouă
    /// </summary>
    public delegate void NewReadingEventHandler(object sender, NewReadingEventArgs e);

    /// <summary>
    /// Argumente pentru evenimentul de citire nouă
    /// </summary>
    public class NewReadingEventArgs : EventArgs
    {
        public Sensor Sensor { get; set; }
        public SensorReading Reading { get; set; }
        public double SimulatedValue { get; set; }
    }

    /// <summary>
    /// Simulator inteligent pentru senzori - EFECT WOW!
    /// Simulează variații sezoniere, zilnice și evenimente aleatorii
    /// </summary>
    public class SensorSimulator
    {
        private System.Timers.Timer _timer;
        private readonly SensorRepository _sensorRepo;
        private readonly AlertService _alertService;
        private readonly Random _random;
        
        // Evenimente WOW!
        public event NewReadingEventHandler NewReadingGenerated;
        public event EventHandler SimulationStarted;
        public event EventHandler SimulationStopped;

        // Proprietăți pentru simulare avansată
        public bool IsRunning { get; private set; }
        public int UpdateIntervalSeconds { get; set; } = 5;
        public double NoiseLevel { get; set; } = 0.1; // 10% zgomot

        public SensorSimulator()
        {
            _sensorRepo = new SensorRepository();
            _alertService = new AlertService();
            _random = new Random();
        }

        /// <summary>
        /// Pornește simularea
        /// </summary>
        public void StartSimulation()
        {
            if (IsRunning) return;

            _timer = new System.Timers.Timer(UpdateIntervalSeconds * 1000);
            _timer.Elapsed += OnSimulationTick;
            _timer.AutoReset = true;
            _timer.Start();
            
            IsRunning = true;
            SimulationStarted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Oprește simularea
        /// </summary>
        public void StopSimulation()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            
            IsRunning = false;
            SimulationStopped?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// La fiecare tick al timer-ului, generează citiri pentru toți senzorii
        /// </summary>
        private async void OnSimulationTick(object sender, ElapsedEventArgs e)
        {
            var sensors = _sensorRepo.GetAllSensors();
            
            foreach (var sensor in sensors)
            {
                // Generează o valoare simulată inteligentă
                double simulatedValue = GenerateSmartValue(sensor);
                
                // Adaugă zgomot pentru realism
                simulatedValue = AddNoise(simulatedValue, sensor);
                
                // Asigură că valoarea rămâne în limitele posibile
                simulatedValue = Math.Max(sensor.MinValue, Math.Min(sensor.MaxValue, simulatedValue));

                var reading = new SensorReading
                {
                    SensorId = sensor.SensorId,
                    Timestamp = DateTime.Now,
                    Value = Math.Round(simulatedValue, 2),
                    Quality = DetermineQuality(simulatedValue, sensor)
                };

                // Salvează citirea
                _sensorRepo.AddReading(reading);

                // Verifică pragurile și generează alerte
                _alertService.CheckThresholds(sensor, reading);

                // Declanșează evenimentul pentru UI
                OnNewReadingGenerated(sensor, reading, simulatedValue);
            }
        }

        /// <summary>
        /// Generează valori inteligente bazate pe tipul senzorului și ora din zi
        /// EFECT WOW: Simulare realistă cu variații sezoniere
        /// </summary>
        private double GenerateSmartValue(Sensor sensor)
        {
            double baseValue = 0;
            DateTime now = DateTime.Now;

            switch (sensor.Type)
            {
                case SensorType.Temperature:
                    // Temperatură: mai cald ziua, mai rece noaptea
                    double hourOfDay = now.Hour + now.Minute / 60.0;
                    // Model sinusoidal: maxim la 14:00, minim la 4:00
                    double dailyVariation = Math.Sin((hourOfDay - 6) * Math.PI / 12) * 5;
                    // Variație sezonieră (vara mai cald, iarna mai rece)
                    int dayOfYear = now.DayOfYear;
                    double seasonalVariation = Math.Sin((dayOfYear - 80) * 2 * Math.PI / 365) * 8;
                    
                    baseValue = 20 + dailyVariation + seasonalVariation;
                    break;

                case SensorType.Humidity:
                    // Umiditate: mai mare noaptea, mai mică ziua
                    hourOfDay = now.Hour;
                    dailyVariation = Math.Cos((hourOfDay - 2) * Math.PI / 12) * 15;
                    baseValue = 60 + dailyVariation;
                    break;

                case SensorType.SoilMoisture:
                    // Umiditate sol: scade în timpul zilei (evaporare)
                    hourOfDay = now.Hour;
                    if (hourOfDay > 10 && hourOfDay < 18)
                        baseValue = 45 - (hourOfDay - 10) * 2;
                    else
                        baseValue = 55;
                    break;

                case SensorType.Light:
                    // Lumină: doar ziua
                    hourOfDay = now.Hour;
                    if (hourOfDay >= 6 && hourOfDay <= 20)
                    {
                        baseValue = 100 * Math.Sin((hourOfDay - 6) * Math.PI / 14);
                    }
                    else
                    {
                        baseValue = 5; // lumină ambientală noaptea
                    }
                    break;

                case SensorType.CO2:
                    // CO2: mai mult noaptea (plantele respiră)
                    hourOfDay = now.Hour;
                    if (hourOfDay < 6 || hourOfDay > 20)
                        baseValue = 450 + _random.Next(-20, 20);
                    else
                        baseValue = 400 + _random.Next(-10, 10);
                    break;

                default:
                    baseValue = _random.NextDouble() * 100;
                    break;
            }

            return baseValue;
        }

        /// <summary>
        /// Adaugă zgomot aleator pentru realism
        /// </summary>
        private double AddNoise(double value, Sensor sensor)
        {
            double noiseRange = (sensor.MaxValue - sensor.MinValue) * NoiseLevel;
            double noise = (_random.NextDouble() * 2 - 1) * noiseRange;
            return value + noise;
        }

        /// <summary>
        /// Determină calitatea citirii
        /// </summary>
        private ReadingQuality DetermineQuality(double value, Sensor sensor)
        {
            if (value < sensor.MinValue || value > sensor.MaxValue)
                return ReadingQuality.Invalid;
            
            if (value < sensor.CriticalLow || value > sensor.CriticalHigh)
                return ReadingQuality.Suspect;
            
            return ReadingQuality.Normal;
        }

        /// <summary>
        /// Declanșează evenimentul de citire nouă
        /// </summary>
        protected virtual void OnNewReadingGenerated(Sensor sensor, SensorReading reading, double simulatedValue)
        {
            NewReadingGenerated?.Invoke(this, new NewReadingEventArgs
            {
                Sensor = sensor,
                Reading = reading,
                SimulatedValue = simulatedValue
            });
        }
    }
}