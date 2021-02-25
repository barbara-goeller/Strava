namespace Strava.Services
{
    using ChartJs.Blazor;
    using ChartJs.Blazor.Common;
    using ChartJs.Blazor.PieChart;
    using ChartJs.Blazor.Util;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class ChartService : IChartService
    {
        public PieConfig DrawPieChart<T>(string title, SortedDictionary<string, T> data)
        {
            var config = new PieConfig
            {
                Options = new PieOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = title
                    },
                    CutoutPercentage = 50,
                    Tooltips = new Tooltips
                    {
                        Enabled = true,
                    },
                    MaintainAspectRatio = false
                }
            };

            var values = new T[data.Count];
            for(int i = 0; i < data.Count; i++)
            {
                var dataElement = data.ElementAt(i);
                config.Data.Labels.Add(dataElement.Key);
                values[i] = dataElement.Value;
            }

            var dataset = new PieDataset<T>(values) { BackgroundColor = ChartColors.All.Take(data.Count).Select(ColorUtil.FromDrawingColor).ToArray() };

            config.Data.Datasets.Add(dataset);

            return config;
        }

        public void UpdatePieChartData<T>(Chart chartToUpdate, PieConfig configToUpdate, SortedDictionary<string, T> data)
        {
            configToUpdate.Data.Labels.Clear();
            configToUpdate.Data.Datasets.Clear();

            var values = new T[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                var dataElement = data.ElementAt(i);
                configToUpdate.Data.Labels.Add(dataElement.Key);
                values[i] = dataElement.Value;
            }

            configToUpdate.Data.Datasets.Add(new PieDataset<T>(values) { BackgroundColor = ChartColors.All.Take(data.Count).Select(ColorUtil.FromDrawingColor).ToArray() });

            chartToUpdate.Update();
        }
    }

    public static class ChartColors
    {
        private static readonly Lazy<IReadOnlyList<Color>> _all = new Lazy<IReadOnlyList<Color>>(() => new Color[7]
        {
                Red, Orange, Yellow, Green, Blue, Purple, Grey
        });

        public static IReadOnlyList<Color> All => _all.Value;

        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.brushes?view=net-5.0
        public static readonly Color Red = Color.Red;
        public static readonly Color Orange = Color.Orange;
        public static readonly Color Yellow = Color.Gold;
        public static readonly Color Green = Color.Green;
        public static readonly Color Blue = Color.Blue;
        public static readonly Color Purple = Color.Purple;
        public static readonly Color Grey = Color.Gray;
    }
}
