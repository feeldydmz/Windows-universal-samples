using Megazone.Cloud.Media.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.Cloud.Media.Repository
{
    public class AuthorizationRepository: IAuthorizationRepository
    {
        public AuthorizationResponse Get(AuthorizationRequest request)
        {
            const string url = "https://megaone.io/oauth/token";
            

        }

        private string getClientAuthorization()
        {
            const string CLIENT_ID = "0a31e7dc-65eb-4430-9025-24f9e3d7d60d";
            const string CLIENT_SECRET_KEY = "02255b95-cb75-4b28-9c0a-30f4afd02f00";

            var authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{CLIENT_ID}:{CLIENT_SECRET_KEY}"));

            return authorization;
        }

    }

    
}
