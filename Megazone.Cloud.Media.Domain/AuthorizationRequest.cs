﻿namespace Megazone.Cloud.Media.Domain
{
    public class AuthorizationRequest
    {
        public AuthorizationRequest(string username, string password, string code)
        {
            Username = username;
            Password = password;
            Code = code;
        }

        public string Username { get; }
        public string Password { get; }
        public string Code { get; }
    }
}