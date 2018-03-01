using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcsbot.ChampionGG
{
    public class Exception : System.Exception
    {
        public ChampionGgErrorCode Code { get; set; }

        public Exception() { }

        public Exception(string message) : base(message) { }

        public Exception(string message, System.Exception innerException) : base(message, innerException) { }

        public static Exception CreateForErrorCode(int code)
        {
            var errorCode = (ChampionGgErrorCode)code;
            var msg = $"{(int)code}: {code}";
            return new Exception(msg) { Code = errorCode };
        }
    }

    public enum ChampionGgErrorCode
    {
        JsonSerializationError = 0,
        BadRequest = 400,
        Forbidden = 403,
        DataNotFound = 404,
        ServerError = 500,
        Unavailable = 503
    }
}
