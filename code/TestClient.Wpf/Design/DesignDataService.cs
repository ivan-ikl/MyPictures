﻿using System;
using TestClient.Wpf.Model;

namespace TestClient.Wpf.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem("Azure MyPictures [design]");
            callback(item, null);
        }
    }
}