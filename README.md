# Mvc.NET Utilities
Miscellaneous ASP.NET MVC utilties 

- DodCac - Get simple information from a CAC. (https://en.wikipedia.org/wiki/Common_Access_Card) See README.txt and screenshot in the Views directory to enable SSL and client certificates in IIS Express / Visual Studio. (tested on 2012/2013)
- ConditionalFilterProvider - MVC controller / action filters
- Cross-site request forgery (XSRF - https://en.wikipedia.org/wiki/Cross-site_request_forgery):
    - FilterConfig.cs hook - ConditionalFilterProvider
    - ValidateJsonAntiForgeryTokenAttribute (XHR w/json payload)
