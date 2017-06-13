/*
 * Henry Gao
 * An enumeration of the server states, which determines
 * how the network will behave (ex. Will it send or wait for messages).
 * Jan 24th, 2017.
*/

public enum ServerState
{
    Waiting, //When the server is waiting for a message
    Authenticating, //When the server is waiting to connect to a client
    Active, //If the server can send a message
    Finished //If the server is disconnected
}
