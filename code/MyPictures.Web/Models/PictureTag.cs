namespace MyPictures.Web.Models
{
    using System;
    using System.Data.Services.Common;
    using MyPictures.Web.Repositories;

    [DataServiceEntity]
    [DataServiceKey(new[] { "PartitionKey", "RowKey" })]
    public class PictureTag : ITableServiceEntity
    {
        private const string PartitionKeyName = "picturetag";

        public Guid PictureId { get; set; }

        public string TagName { get; set; }

        public string PartitionKey
        {
            get { return PartitionKeyName; }
            set { }
        }

        public string RowKey
        {
            get { return this.TagName + ";" + this.PictureId; }
            set { }
        }

        public DateTime Timestamp { get; set; }
    }
}