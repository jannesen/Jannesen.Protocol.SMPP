# Jannesen.Protocol.SMPP

This library implements a basis Short Message Peer-to-Peer protocol. It only implements sending SMS messages and receiving delivery reports.

The library is complete async.

## SMPPConnection

### properties

| name           | description
|:---------------|:-------
| Hostname       | Hostname of the SMPP server to connect to.
| Post           | Port to connect to
| Tls            | Use a TLS connection
| SMPPBind       | Get a object to to SMPP bind data. Needed to set username/password etc.
| Url            | [smpp|smpps]://&lt;hostname&gt;:&lt;port&gt;
| State          | Current connection state
| ActiveRequests | Number of SMS message in the queue to send to SMPP gateway
| OnStateChange  | callback delegate when the state changes
| OnDeliverSm    | callback delegate on reception of delivery report.


### methods

| name         | description
|:-------------|:-------
| ConnectAsync | Connect to SMPP gateway 
| StopAsync    | Stop (gracefully)
| Close        | Close the connection immediately and release all resources.
