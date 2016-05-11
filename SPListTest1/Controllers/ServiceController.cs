using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.SharePoint.Client;

namespace SPListTest1.Controllers
{
    public class ServiceController : ApiController
    {
        string SiteUrl = "http://win08vm/cysun";

        [HttpGet]
        public HttpResponseMessage EnsureUser( string username )
        {
            ClientContext clientContext = new ClientContext(SiteUrl);
            var user = clientContext.Web.EnsureUser(username);
            clientContext.Load(user);
            try
            {
                clientContext.ExecuteQuery();
            }
            catch( ServerException e )
            {
                user = null;
            }

            return user != null ? new HttpResponseMessage(HttpStatusCode.OK) :
                new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}
