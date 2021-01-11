using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using TheProcessE;

namespace RestApiClient.NetStandardTests
{
    public class AuthModel : IAuthModel
    {

        public AuthenticationHeaderValue Authorization { get; private set; }
        public AuthModel()
        {
        }
        public IAuthModel Create() => new AuthModel 
        {
            Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJBY2NvdW50VHlwZSI6IkJ1c2luZXNzIiwiTmFtZSI6IlRoZSBQcm9jZXNzIEVudGVycHJpc2UiLCJJZCI6IjhjZWMxMjdkLTI5YWEtNDFjMy1hOWQyLTNiZjhkZmNlNTA2YSIsIlRpbWVTdGFtcCI6IjAzLzI5LzIwMjAgMjM6MDY6MDIiLCJFbWFpbCI6InRoZXByb2Nlc3NkbUBnbWFpbC5jb20iLCJuYmYiOjE1ODU1Mjk1OTYsImV4cCI6MTU4NjEzNDM5NiwiaWF0IjoxNTg1NTI5NTk2fQ.MnHP1C1CiHQ6CaVCuJn2qZRO_Rp18JyFIO_7rd7eHdyF68J3KxxzfC9m9toGg1GkqI2GOIp94bEj7chxT1oZQw")
        };
    }
}
