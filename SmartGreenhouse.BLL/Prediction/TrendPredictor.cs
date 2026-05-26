using System;
using System.Collections.Generic;
using System.Linq;
using SmartGreenhouse.Models.Entities;

namespace SmartGreenhouse.BLL.Prediction
{
    /// <summary>
    /// EFECT WOW: Predicție simplă a tendințelor folosind regresie liniară
    /// </summary>
    public class TrendPredictor
    {
        /// <summary>
        /// Predice valoarea următoare bazată pe istoric
        /// </summary>
        public double PredictNextValue(List<SensorReading> readings)
        {
            if (readings == null || readings.Count < 2)
                return 0;

            // Folosim doar ultimele 10 citiri pentru predicție
            var recentReadings = readings.OrderBy(r => r.Timestamp)
                                         .TakeLast(10)
                                         .ToList();

            // Regresie liniară simplă
            int n = recentReadings.Count;
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            for (int i = 0; i < n; i++)
            {
                double x = i; // indexul în timp
                double y = recentReadings[i].Value;

                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            double intercept = (sumY - slope * sumX) / n;

            // Predicție pentru următorul pas (index = n)
            double predictedValue = slope * n + intercept;

            return Math.Round(predictedValue, 2);
        }

        /// <summary>
        /// Determină tendința (creștere, descreștere, stabil)
        /// </summary>
        public string GetTrend(List<SensorReading> readings)
        {
            if (readings == null || readings.Count < 5)
                return "Insufficient data";

            var lastFive = readings.OrderByDescending(r => r.Timestamp)
                                   .Take(5)
                                   .Select(r => r.Value)
                                   .ToList();

            double firstAvg = lastFive.Take(2).Average();
            double lastAvg = lastFive.Skip(3).Average();

            double difference = lastAvg - firstAvg;
            double percentChange = Math.Abs(difference / firstAvg) * 100;

            if (percentChange < 1)
                return "Stable";
            else if (difference > 0)
                return $"Rising by {percentChange:F1}%";
            else
                return $"Falling by {percentChange:F1}%";
        }
    }
}