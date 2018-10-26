using System;

namespace CSharpSampleApp.Entities
{
    [Serializable]
    public class Server
    {
        public enum Status : byte
        {
            Unknown = 0,

            Online = 1,

            Offline = 2
        }
    }
}