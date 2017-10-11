namespace ServiceBase.Collections
{
    public class SortInfo
    {
        public string Field { get; set; }
        public SortDirection Direction { get; set; }

        public SortInfo()
        {
        }

        public SortInfo(string field, SortDirection direction)
        {
            this.Field = field;
            this.Direction = direction;
        }
    }
}