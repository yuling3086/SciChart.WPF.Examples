﻿// *************************************************************************************
// SCICHART® Copyright SciChart Ltd. 2011-2017. All rights reserved.
//  
// Web: http://www.scichart.com
//   Support: support@scichart.com
//   Sales:   sales@scichart.com
// 
// HeatMapExampleView.xaml.cs is part of the SCICHART® Examples. Permission is hereby granted
// to modify, create derivative works, distribute and publish any part of this source
// code whether for commercial, private or personal use. 
// 
// The SCICHART® examples are distributed in the hope that they will be useful, but
// without any warranty. It is provided "AS IS" without warranty of any kind, either
// expressed or implied. 
// *************************************************************************************
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Model.DataSeries.Heatmap2DArrayDataSeries;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Core.Helpers;

namespace SciChart.Examples.Examples.HeatmapChartTypes.RealTimeHeatmap
{
    /// <summary>
    /// demonstrates use of FastHeatmapRenderableSeries
    /// creates a list of Heatmap2dArrayDataSeries and displays them one by one in loop on timer
    /// </summary>
    public partial class HeatMapExampleView : UserControl
    {
        /// <summary>
        /// list of data series to be displayed in loop on timer
        /// </summary>
        private readonly IDataSeries[] _dataSeries = new IDataSeries[seriesPerPeriod];
        /// <summary>
        /// number of series to be displayed in a loop (period)
        /// </summary>
        private const int seriesPerPeriod = 30;
        private DispatcherTimer _timer;
        private int _timerIndex = 0;

        public HeatMapExampleView()
        {
            InitializeComponent();

            var colormap = heatmapSeries.ColorMap;
            var cpMin = colormap.Minimum;
            var cpMax = colormap.Maximum;
            // Create data for our heatmaps in parallel
            Parallel.For(0, seriesPerPeriod, (i) =>
            {
                _dataSeries[i] = CreateSeries(i, 300, 200, cpMin, cpMax);
            });

            heatmapSeries.DataSeries = _dataSeries[0];

            _timer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(1),
#if !SILVERLIGHT
                IsEnabled = true
#endif
            };
#if SILVERLIGHT
            _timer.Start();
#endif
            _timer.Tick += TimerOnTick;
        }

        /// <summary>
        /// displays next data series in list
        /// </summary>
        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            _timerIndex++;
            heatmapSeries.DataSeries = _dataSeries[_timerIndex % _dataSeries.Length];
        }

        private IDataSeries CreateSeries(int index, int width, int height, double cpMin, double cpMax)
        {
            var random = new Random(Environment.TickCount << index);
            double angle = Math.PI * 2 * index / seriesPerPeriod;
            int w = width, h = height;
            var data = new double[h, w];
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    var v = (1 + Math.Sin(x * 0.04 + angle)) * 50 + (1 + Math.Sin(y * 0.1 + angle)) * 50 * (1 + Math.Sin(angle * 2));
                    var cx = w / 2; var cy = h / 2;
                    var r = Math.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                    var exp = Math.Max(0, 1 - r * 0.008);
                    var zValue = (v * exp + random.NextDouble() * 50);
                    data[y, x] = (zValue > cpMax) ? cpMax : zValue;
                }
            return new UniformHeatmapDataSeries<int, int, double>(data, 0, 1, 0, 1);
        }

        private void OnExampleLoaded(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }

        private void OnExampleUnloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }
    }
}
