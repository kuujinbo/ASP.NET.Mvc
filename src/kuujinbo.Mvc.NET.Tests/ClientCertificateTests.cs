using kuujinbo.Mvc.NET.Tests.Properties;
using Moq;
using System;
using System.Collections.Specialized;
using System.Web;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests
{
    public class ClientCertificateTests
    {
        const string SelfMadeCertificateSubject = "last.first.middle.0987654321";
        byte[] _fakeCertificateBytes;
        ClientCertificate _clientCertificate;
        Mock<HttpRequestBase> _request;

        public ClientCertificateTests()
        {
            _fakeCertificateBytes = new byte[20];
            _request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var httpWorkerRequest = new Mock<HttpWorkerRequest>();
            httpWorkerRequest.Setup(x => x.GetRawUrl()).Returns("/");
            httpWorkerRequest.Setup(x => x.GetClientCertificate()).Returns(_fakeCertificateBytes);
            HttpContext context = new HttpContext(httpWorkerRequest.Object);
            _request.Setup(x => x.ClientCertificate).Returns(context.Request.ClientCertificate);
            _clientCertificate = new ClientCertificate();
        }

        [Fact]
        public void GetCertificate_IsLocalRequest_ReturnsByteArrayFromRequestCertificate()
        {
            _request.Setup(x => x.IsLocal).Returns(true);

            Assert.IsType<byte[]>(_clientCertificate.GetCertificate(_request.Object));
        }

        [Fact]
        public void GetCertificate_IsNotLocalRequest_ReturnsByteArrayFromRequestHeaders()
        {
            _request.Setup(x => x.IsLocal).Returns(false);

            var headers = new NameValueCollection();
            headers[ClientCertificate.BigIpCertificateHeader] = Convert
                .ToBase64String(_fakeCertificateBytes);
            _request.Setup(x => x.Headers).Returns(headers);

            var result = _clientCertificate.GetCertificate(_request.Object);

            Assert.IsType<byte[]>(result);
            Assert.Equal(_fakeCertificateBytes.Length, result.Length);
        }

        [Fact]
        public void GetCacUser_NoChainValidation_ReturnsCacUser()
        {
            _request.Setup(x => x.IsLocal).Returns(false);

            var headers = new NameValueCollection();
            headers[ClientCertificate.BigIpCertificateHeader] = Convert
                .ToBase64String(Resources.Cac);
            _request.Setup(x => x.Headers).Returns(headers);

            var result = _clientCertificate.GetCacUser(_request.Object);

            Assert.IsType<CacUser>(result);
            Assert.Equal("Last", result.LastName);
            Assert.Equal("First", result.FirstName);
            Assert.Equal("Middle", result.MiddleName);
            Assert.Equal("0987654321", result.Edipi);
            Assert.Equal("CN=Root Agency", result.Issuer);
            Assert.Equal("email@domain", result.Email);
            Assert.Equal(SelfMadeCertificateSubject, result.Subject);
            Assert.Null(result.ChainError);
        }

        [Fact]
        public void GetCacUser_ChainValidation_ReturnsCacUserWithChainErrorSet()
        {
            _request.Setup(x => x.IsLocal).Returns(false);

            var headers = new NameValueCollection();
            headers[ClientCertificate.BigIpCertificateHeader] = Convert
                .ToBase64String(Resources.Cac);
            _request.Setup(x => x.Headers).Returns(headers);

            var result = _clientCertificate.GetCacUser(_request.Object, true);

            Assert.IsType<CacUser>(result);
            Assert.Equal("Last", result.LastName);
            Assert.Equal("First", result.FirstName);
            Assert.Equal("Middle", result.MiddleName);
            Assert.Equal("0987654321", result.Edipi);
            Assert.Equal("CN=Root Agency", result.Issuer);
            Assert.Equal("email@domain", result.Email);
            Assert.Equal(SelfMadeCertificateSubject, result.Subject);
            Assert.NotNull(result.ChainError);
        }
    }
}