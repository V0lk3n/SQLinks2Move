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

## Demo

TODO


