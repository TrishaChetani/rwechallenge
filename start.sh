#!/bin/sh
while ! nc -z rabbitmq 15672; do sleep 3; done
npm test
