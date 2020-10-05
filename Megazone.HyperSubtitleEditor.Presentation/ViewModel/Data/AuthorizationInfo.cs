using System;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    public class AuthorizationInfo : Authorization
    {
        public AuthorizationInfo(string accessToken, string refreshToken, string expires, DateTime expiresTime)
            : base(accessToken, refreshToken, expires)
        {
            /*Authorization = authorization;
            ExpiresTime = expireTime;*/
            ExpiresTime = expiresTime;
        }

//        public AuthorizationInfo(string accessToken, string refreshToken, string expires, string expiresTime)
//            : base(accessToken, refreshToken, expires)
//        {
//            /*Authorization = authorization;
//            ExpiresTime = expireTime;*/
//            ExpiresTime = DateTime.Parse(expireTime);
//        }

        //        public Authorization Authorization { get; private set; }
        public DateTime ExpiresTime { get; set; }
    }
}