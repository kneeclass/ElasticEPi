using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticEPi.Test.EPiServerInitialization;
using EPiServer.ServiceLocation;
using Nest;
using Xunit;

namespace ElasticEPi.Test
{
    public class FilterTests : IClassFixture<EPiServerInitializer> {
        public FilterTests(EPiServerInitializer initializer)
        {
            var applicationPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            initializer.Initialize(applicationPath);
        }


    }
}
