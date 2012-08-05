// ---------------------------------------------------------------------------------- 
// Microsoft Developer & Platform Evangelism 
//  
// Copyright (c) Microsoft Corporation. All rights reserved. 
//  
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,  
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES  
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
// ---------------------------------------------------------------------------------- 
// The example companies, organizations, products, domain names, 
// e-mail addresses, logos, people, places, and events depicted 
// herein are fictitious.  No association with any real company, 
// organization, product, domain name, email address, logo, person, 
// places, or events is intended or should be inferred. 
// ---------------------------------------------------------------------------------- 

namespace MyPictures.Web.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StorageEmulatorInitializer
    {
        [AssemblyInitialize]
        public static void DevelopmentStorageInitialize(TestContext context)
        {
            var sdkDirectory = Path.Combine(Environment.GetEnvironmentVariable("ProgramW6432"), @"Windows Azure Emulator\emulator");
            var processStartInfo = new ProcessStartInfo()
            {
                FileName = Path.Combine(sdkDirectory, "csrun.exe"),
                Arguments = "/devstore",
            };

            using (var process = Process.Start(processStartInfo))
            {
                process.WaitForExit();
            }
        }
    }
}
