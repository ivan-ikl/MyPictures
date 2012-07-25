namespace MyPictures.Web.Models
{
    using System;
    using System.Data.Services.Common;
    using MyPictures.Web.Repositories;

    [DataServiceEntity]
    [DataServiceKey(new[] { "PartitionKey", "RowKey" })]
    public class Tag : ITableServiceEntity
    {
        private const string PartitionKeyName = "tags";

        public string Name { get; set; }

        public int PicturesCount { get; set; }

        public string PartitionKey
        {
            get { return PartitionKeyName; }
            set { }
        }

        public string RowKey
        {
            get { return this.Name; }
            set { this.Name = value; }
        }

        public DateTime Timestamp { get; set; }
    }
}
