module.exports = {
  good: {
    url: process.env.AMQP_URL || 'amqp://guest:guest@localhost:15672',
    exchange: 'amq.topic',
    queue: {
      name: 'turbine-updates',
      routingKey: 'myRoutingQueue',
      options: { deadLetterExchange: 'wow' }
    }
  },
  noRoutingKey: {
    url: process.env.AMQP_URL || 'amqp://guest:guest@localhost:15672',
    exchange: 'hasone',
    queue: {
      name: 'myconsumequeue',
      options: {}
    }
  },
  routingKeyArray: {
    url: process.env.AMQP_URL || 'amqp://guest:guest@localhost:15672',
    exchange: 'mytestexchange',
    queue: {
      name: 'myconsumequeue',
      routingKey: ['myRoutingKey', 'myRoutingKey2'],
      options: { deadLetterExchange: 'wow' }
    }
  }
};

// vim: set et sw=2:
