namespace MyPictures.Web.Models
{
    using System;
    using System.Data.Services.Common;
    using System.Linq;
    using MyPictures.Web.Repositories;

    [DataServiceEntity]
    [DataServiceKey(new[] { "PartitionKey", "RowKey" })]
    public class Picture : ITableServiceEntity
    {
        private const string PartitionKeyName = "pictures";

        private string tags;

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Uri Url { get; set; }

        public string Tags
        {
            get { return this.tags; }
            set { this.tags = string.Join(",", value.Split(',').Select(t => t.Trim()).Distinct()); }
        }

        public string PartitionKey
        {
            get { return PartitionKeyName; }
            set { }
        }

        public string RowKey
        {
            get { return this.Id.ToString(); }
            set { this.Id = new Guid(value); }
        }

        public DateTime Timestamp { get; set; }
    }
}
