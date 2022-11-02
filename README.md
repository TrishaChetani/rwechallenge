# QA engineer challenge

## RabbitMQ setup

To connect to RabbitMQ configure:

- `RabbitMQ:Connections:Default:HostName`
- `RabbitMQ:Connections:Default:Username`
- `RabbitMQ:Connections:Default:Password`

Queues and bindings will be created automatically (if necessary).
Inputs & outputs are defined in the `events.yaml` (based on [AsyncAPI](https://www.asyncapi.com)).

## Master data API

The base URI for the master data API will be loaded from the config via:

- `MasterData:BaseUri`

The client is auto-generated from the OpenAPI spec.
