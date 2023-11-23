namespace Football.UI.Helpers
{
    public class TableSort
    {
        private bool IsSortedAscending { get; set; }
        private string CurrentSortColumn { get; set; } = "";
        public List<T> SortTable<T>(List<T> table, string columnName)
        {
            var property = typeof(T).GetProperties().FirstOrDefault(p => p.ToString()!.Contains(columnName));
            if (property != null)
            {
                if (columnName != CurrentSortColumn)
                {
                    CurrentSortColumn = columnName;
                    IsSortedAscending = true;
                    return table.OrderBy(t => property.GetValue(t, null)).ToList();
                }
                else
                {                   
                    if (IsSortedAscending)
                    {
                        IsSortedAscending = false;
                        return table.OrderByDescending(t => property.GetValue(t, null)).ToList();
                    }
                    else
                    {
                        IsSortedAscending = true;
                        return table.OrderBy(t => property.GetValue(t, null)).ToList();
                    }
                }
            }
            return table;
        }

        public string GetSortStyle(string columnName)
        {
            if (CurrentSortColumn != columnName)
            {
                return string.Empty;
            }
            return IsSortedAscending ? "oi-caret-top" : "oi-caret-bottom";           
        }
    }
}
