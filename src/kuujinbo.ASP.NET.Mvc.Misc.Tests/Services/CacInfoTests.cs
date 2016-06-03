using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kuujinbo.ASP.NET.Mvc.Misc.Services;
using kuujinbo.ASP.NET.Mvc.Misc.Tests.Properties;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests.Services
{
    public class CacInfoTests
    {
        [Fact]
        public void Get_NullRawData_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => CacInfo.Get(null) 
             );

            Assert.Equal<string>(CacInfo.NULL_GET_PARAM, exception.ParamName);
        }

        [Fact]
        public void Get_ValidRawDataCertificate_ReturnsCacPopulatedCacInfo()
        {
            var cacInfo = CacInfo.Get(Resources.CacInfo_cert);

            Assert.Equal<string>("last", cacInfo.LastName);
            Assert.Equal<string>("first", cacInfo.FirstName);
            Assert.Equal<string>("0987654321", cacInfo.Edipi);
            Assert.Equal<string>("email@domain", cacInfo.Email);
        }
        
    }
}