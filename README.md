# End to end encrypted chat program
This is an ongoing project I sometimes work on in my spare time while studying at the University of Bath.

It features a frontend built in WPF and backend in C#. 
<hr></hr>

## Current State
With the server you can:
* Have multiple clients connected simultaniously
* Distribute messages to all connected clients

With the client you can:
* Connect to a server
* Display messages from connected clients*
* Send and receive encrypted messages*

*Currently encryption keys are stored locally and only for the session, meaning only the user sending the message can read the message. This is temporary and is planned to be fixed in the future (see below)
<hr></hr>

## Future Plans 
Server:
* Support x509 certificates
* Distribute public keys from clients
* Login system for restricted chats

Client:
* Decrypt messages from multiple users
* Send and receive Images & Audio 
* Persistently store messages accross startups
* Stay connected to multiple chat rooms at once
* Login
