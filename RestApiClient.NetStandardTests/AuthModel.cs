using System;
using System.Collections.Generic;
using System.Text;
using TheProcessE;

namespace RestApiClient.NetStandardTests
{
    public class AuthModel : IAuthModel
    {
        public string Scheme { get; private set; } 

        public string Token { get; private set; }
        public AuthModel()
        {
        }
        string t = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJBY2NvdW50VHlwZSI6IkJ1c2luZXNzIiwiTmFtZSI6IkZpcnN0NFJlYXNvbiIsIklkIjoiZmUxNThhNDQtMTIwMy00MmUyLWFkYWYtMTRhN2Y1Y2FhZDU2IiwiVGltZVN0YW1wIjoiMDYvMDMvMjAyMCAxNDo1NDo1MiIsIkVtYWlsIjoiYmVub2l0dGhlZmlyc3RAZ21haWwuY29tIiwibmJmIjoxNTg0MzkxNTAzLCJleHAiOjE1ODQ5OTYzMDMsImlhdCI6MTU4NDM5MTUwM30._klX0oLjnR0FYvL5W397RRrlmrl27Gvi7iN-_No9NhZycIhDO7tl8X2XZdKN61IqmqKEqCQLSbQLnFN26iHINA";
        public IAuthModel Create() => new AuthModel 
        {
            Scheme = "Bearer",
            Token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJBY2NvdW50VHlwZSI6IkJ1c2luZXNzIiwiTmFtZSI6IlRoZSBQcm9jZXNzIEVudGVycHJpc2UiLCJJZCI6IjhjZWMxMjdkLTI5YWEtNDFjMy1hOWQyLTNiZjhkZmNlNTA2YSIsIlRpbWVTdGFtcCI6IjAzLzI5LzIwMjAgMjM6MDY6MDIiLCJFbWFpbCI6InRoZXByb2Nlc3NkbUBnbWFpbC5jb20iLCJuYmYiOjE1ODU1Mjk1OTYsImV4cCI6MTU4NjEzNDM5NiwiaWF0IjoxNTg1NTI5NTk2fQ.MnHP1C1CiHQ6CaVCuJn2qZRO_Rp18JyFIO_7rd7eHdyF68J3KxxzfC9m9toGg1GkqI2GOIp94bEj7chxT1oZQw"
        };
    }
}
