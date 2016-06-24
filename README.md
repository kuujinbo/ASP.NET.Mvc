# ASP.NET.Mvc
Miscellaneous ASP.NET MVC code

- CacInfo - Get simple information from a CAC. (https://en.wikipedia.org/wiki/Common_Access_Card) See README.txt and screenshot  in the Views directory to enable SSL and client certificates in IIS Express / Visual Studio. (tested on 2012/2013)
- ConditionalFilterProvider - MVC controller / action filters
- Simple Json.NET (http://www.newtonsoft.com/json) `bool` and `enum` converters.
- Simple Json.NET `ContentResult` (https://msdn.microsoft.com/en-us/library/system.web.mvc.contentresult.aspx)
- LINQ to Entities jQuery DataTables - https://datatables.net/
- Simple OpenXml Excel Writer - https://en.wikipedia.org/wiki/Office_Open_XML
- WebAPI Ajax Binary File Download
- Cross-site request forgery (XSRF - https://en.wikipedia.org/wiki/Cross-site_request_forgery):
    - AngularJS - custom request interceptor
    - FilterConfig.cs hook - ConditionalFilterProvider
    - ValidateJsonAntiForgeryTokenAttribute (XHR w/json payload)
- AngularJS directive - Plupload - http://www.plupload.com/