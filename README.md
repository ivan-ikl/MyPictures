## MyPictures Sample

MyPictures is a ASP.NET Web API sample application for exposing Pictures in Blob storage. It also has a simple web interface to consume the services.

The sample was built using [ASP.NET MVC 4](http://www.asp.net/mvc/mvc4), [ASP.NET Web API](http://www.asp.net/web-api), [jQuery](http://jquery.com/) and [Windows Azure Storage](http://www.windowsazure.com/en-us/home/features/storage/) for the underlying data store.

### Prerequisites
* [Visual Studio 2012](http://www.microsoft.com/visualstudio/11/)
* [ASP.NET MVC 4](http://www.asp.net/mvc/mvc4)
* [Windows Azure Libraries & SDK for .NET 1.7 - June 2012](http://www.windowsazure.com/en-us/develop/downloads/)

### Running the Sample Locally
1. Start the Windows Azure Storage Emulator.
2. Open Visual Studio 2012.
3. Compile the solution. The NuGet packages dependencies will be automatically downloaded and installed.
4. Set the **MyPictures.Web** as startup project and press F5.

### Running the Sample in Windows Azure
1. To run this sample on the cloud you need a Windows Azure Subscription. If you don't have a Windows Azure account, you can sign up for a free trial [here](http://bit.ly/windowsazuretrial).
2. This sample requires a storage account for storing pictures and tags. When running the sample on the cloud you need to make the sample work against a cloud storage account.
3. Open **Web.config** from the **MyPictures.Web** project. Replace the placeholder in the  (**WAZStorageAccount**) appSetting value for the storage account with the values obtained previously from Windows Azure's portal.

### Detailed Instructions ###
A set of detailed instructions on how to set up the sample can be accessed on the sample's [GitHub.com repository page](https://github.com/WindowsAzure-Samples/MyPictures/blob/master/SAMPLE.md "Detailed Sample Walk-through").
