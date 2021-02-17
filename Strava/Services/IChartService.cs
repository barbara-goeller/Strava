namespace Strava.Services
{
    using ChartJs.Blazor;
    using ChartJs.Blazor.PieChart;
    using System.Collections.Generic;

    public interface IChartService
    {
        PieConfig DrawPieChart<T>(string title, SortedDictionary<string, T> data);

        void UpdatePieChartData<T>(Chart chartToUpdate, PieConfig configToUpdate, SortedDictionary<string, T> data);
    }
}
