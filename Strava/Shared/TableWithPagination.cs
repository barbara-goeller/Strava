namespace Strava.Shared
{
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Generic component to show a table with pagination for a list of input items
    /// </summary>
    public partial class TableWithPagination<T>
    {
        [Parameter]
        public IEnumerable<T> InputList { get; set; }

        [Parameter]
        public RenderFragment HeaderDisplay { get; set; }

        [Parameter]
        public RenderFragment<T> ItemDisplay { get; set; }

        [Parameter]
        public int ItemsPerPage { get; set; } = 10;

        private int CurrentPage = 1;
        private List<T> CurrentDisplay;
        private int TotalCount;

        protected override void OnParametersSet()
        {
            UpdateDisplay();
            TotalCount = InputList.Count();
        }

        private void UpdateDisplay()
        {
            CurrentDisplay = InputList.Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
        }

        private bool AtLastPage()
        {
            return CurrentPage >= TotalPages();
        }

        private int TotalPages()
        {
            return Convert.ToInt32(Math.Ceiling(TotalCount / Convert.ToDecimal(ItemsPerPage)));
        }

        private void MoveFirst()
        {
            CurrentPage = 1;
            UpdateDisplay();
        }

        private void MoveBack()
        {
            CurrentPage--;
            UpdateDisplay();
        }

        private void MoveNext()
        {
            CurrentPage++;
            UpdateDisplay();
        }

        private void MoveLast()
        {
            CurrentPage = TotalPages();
            UpdateDisplay();
        }
    }
}
