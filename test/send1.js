#!/usr/bin/env node

var amqp = require('amqplib/callback_api');

const EXCHANGE = 'simple_exchange',
  EXCHANGE_TYPE = 'direct',
  QUEUE = 'turbine-updates',
  ROUTING_KEY = 'simple_routing_key';
  MESSAGE = 'Hello World!';

const defaultPublishCount = 10000;

let runInit = async() => {
  let connection = await amqp.connect('amqp://guest:guest@localhost:15672/');

  let channel = await connection.createChannel();

  let commonOptions = {
    durable: false
  };

  await channel.assertExchange(EXCHANGE, EXCHANGE_TYPE, commonOptions);
  await channel.assertQueue(QUEUE, commonOptions);
  channel.sendToQueue(QUEUE, commonOptions, Buffer.from(MESSAGE));
  console.log(" [x] Sent %s", MESSAGE);
  await channel.bindQueue(QUEUE, EXCHANGE, ROUTING_KEY, commonOptions);
  await channel.close();

  return connection;
};

module.exports = {
  runInit,

};
