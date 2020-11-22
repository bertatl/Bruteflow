﻿using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Bruteflow.Kafka.Consumers.Abstract
{
    internal class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>
    {
        protected IConsumer<TKey, TValue> Consumer;

        public KafkaConsumer(string topic, IConsumer<TKey, TValue> consumer)
        {
            Consumer = consumer;
            Consumer.Subscribe(topic);
        }

        public virtual async Task<ConsumeResult<TKey, TValue>> Consume(CancellationToken cancellationToken)
        {
            await Task.Yield();
            var consumeResult = Consumer.Consume(cancellationToken);
            return consumeResult;
        }

        public Task Close()
        {
            Consumer.Close();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            Consumer?.Dispose();
            Consumer = null;
            return new ValueTask(Task.CompletedTask);
        }
    }
}