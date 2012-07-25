namespace MyPictures.Web.Repositories
{
    using System;

    public interface ITableServiceEntity
    {
        string PartitionKey { get; set; }

        string RowKey { get; set; }

        DateTime Timestamp { get; set; }
    }
}