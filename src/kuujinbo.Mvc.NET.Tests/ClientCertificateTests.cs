using System;
using System.Collections.Specialized;
using System.Web;
using kuujinbo.Mvc.NET.Tests.Properties;
using Moq;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests
{
    public class ClientCertificateTests
    {
        const string SelfMadeCertificateSubject = "last.first.middle.0987654321";
        ClientCertificate _clientCertificate;
        Mock<HttpRequestBase> _httpRequestBase;
        byte[] _fakeCertificateBytes;

        public ClientCertificateTests()
        {
            _httpRequestBase = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var httpWorkerRequest = new Mock<HttpWorkerRequest>();
            httpWorkerRequest.Setup(x => x.GetRawUrl()).Returns("/");
            httpWorkerRequest.Setup(x => x.GetClientCertificate()).Returns(_fakeCertificateBytes);
            HttpContext context = new HttpContext(httpWorkerRequest.Object);
            _httpRequestBase.Setup(x => x.ClientCertificate).Returns(context.Request.ClientCertificate);
            _clientCertificate = new ClientCertificate(_httpRequestBase.Object);
            _fakeCertificateBytes = new byte[20];
        }

        [Fact]
        public void GetCertificate_IsLocalRequest_ReturnsByteArrayFromRequestCertificate()
        {
            _httpRequestBase.Setup(x => x.IsLocal).Returns(true);

            Assert.IsType<byte[]>(_clientCertificate.GetCertificate());
        }

        [Fact]
        public void GetCertificate_IsNotLocalRequest_ReturnsByteArrayFromRequestHeaders()
        {
            _httpRequestBase.Setup(x => x.IsLocal).Returns(false);

            var headers = new NameValueCollection();
            headers[ClientCertificate.BigIpCertificateHeader] = Convert
                .ToBase64String(_fakeCertificateBytes);
            _httpRequestBase.Setup(x => x.Headers).Returns(headers);

            var result = _clientCertificate.GetCertificate();
            Assert.IsType<byte[]>(result);
            Assert.Equal(_fakeCertificateBytes.Length, result.Length);
        }

        [Fact]
        public void GetCacUser_NoChainValidation_ReturnsCacUser()
        {
            _httpRequestBase.Setup(x => x.IsLocal).Returns(false);

            var headers = new NameValueCollection();
            headers[ClientCertificate.BigIpCertificateHeader] = Convert
                .ToBase64String(Resources.Cac);
            _httpRequestBase.Setup(x => x.Headers).Returns(headers);

            var result = _clientCertificate.GetCacUser();
            Assert.IsType<CacUser>(result);
            Assert.Equal<string>("Last", result.LastName);
            Assert.Equal<string>("First", result.FirstName);
            Assert.Equal<string>("Middle", result.MiddleName);
            Assert.Equal<string>("0987654321", result.Edipi);
            Assert.Equal<string>("email@domain", result.Email);
            Assert.Equal<string>(SelfMadeCertificateSubject, result.Subject);
            Assert.Null(result.ChainError);
        }

        [Fact]
        public void GetCacUser_ChainValidation_ReturnsCacUserWithChainErrorSet()
        {
            _httpRequestBase.Setup(x => x.IsLocal).Returns(false);

            var headers = new NameValueCollection();
            headers[ClientCertificate.BigIpCertificateHeader] = Convert
                .ToBase64String(Resources.Cac);
            _httpRequestBase.Setup(x => x.Headers).Returns(headers);

            var result = _clientCertificate.GetCacUser(true);

            Assert.IsType<CacUser>(result);
            Assert.Equal<string>("Last", result.LastName);
            Assert.Equal<string>("First", result.FirstName);
            Assert.Equal<string>("Middle", result.MiddleName);
            Assert.Equal<string>("0987654321", result.Edipi);
            Assert.Equal<string>("email@domain", result.Email);
            Assert.Equal<string>(SelfMadeCertificateSubject, result.Subject);
            Assert.NotNull(result.ChainError);
        }
    }
}