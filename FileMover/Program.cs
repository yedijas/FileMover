using System;
using System.Configuration;
using System.IO;
using System.Security.Permissions;
using FileMoverLib;

namespace FileMover
{
    internal class Program
    {
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        static void Main(string[] args)
        {
            string domainName = ConfigurationManager.AppSettings["domain"];
            string userName = ConfigurationManager.AppSettings["username"];
            string password = ConfigurationManager.AppSettings["password"];
            string from = ConfigurationManager.AppSettings["from"];
            string to = ConfigurationManager.AppSettings["to"];

            FileMoverLib.FileMover.ImpersonateAndMove(domainName, userName, password, from, to);
        }
    }
}
