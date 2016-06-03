using System;
using kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests.Services.JqueryDataTables
{
    public class ViewAllPathTests
    {
        [Fact]
        public void All_WithNullUri_ReturnsFalse()
        {
            Assert.False(ViewAllPath.All(null));
        }

        [Fact]
        public void All_WithOutViewAllSegmentInUri_ReturnsTrue()
        {
            Assert.False(ViewAllPath.All(new Uri("http://test.test")));
        }

        [Fact]
        public void All_WithViewAllSegmentInUri_ReturnsTrue()
        {
            var uri = new Uri(new Uri("http://test.test"), ViewAllPath.SEGMENT);
            Assert.True(ViewAllPath.All(uri));
        }

        //[Fact]
        //public void MakeUrl_WithoutControllerName_ReturnsBasePathPlusSegment()
        //{
        //    var mock = new Moq.Mock<HttpRuntimeWrapper>();
        //    mock.Setup(fake => fake.AppDomainAppVirtualPath).Returns("/");

        //    Assert.Equal(
        //        string.Format("/{0}", ViewAllPath.SEGMENT), 
        //        new ViewAllPath(mock.Object).MakeUrl()
        //    );
        //}

        //[Fact]
        //public void MakeUrl_WithControllerName_ReturnsBasePathControllerNameSegment()
        //{
        //    var name = "controllerName";
        //    var mock = new Moq.Mock<HttpRuntimeWrapper>();
        //    mock.Setup(fake => fake.AppDomainAppVirtualPath).Returns("/");

        //    Assert.Equal(
        //        string.Format("/{0}/{1}", name, ViewAllPath.SEGMENT),
        //        new ViewAllPath(mock.Object).MakeUrl(name)
        //    );
        //}
    }
}