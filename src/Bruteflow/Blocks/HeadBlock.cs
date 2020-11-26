﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     Starting block of a pipeline
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public sealed class HeadBlock<TInput> : IHeadBlock<TInput>, IProducerBlock<TInput>
    {
        private readonly Func<CancellationToken, Func<CancellationToken, TInput, PipelineMetadata, Task>, Task> _process;
        private IReceiverBlock<TInput> _following;

        public HeadBlock() : this(null)
        {
        }

        public HeadBlock(Func<CancellationToken, Func<CancellationToken, TInput, PipelineMetadata, Task>, Task> process)
        {
            _process = process;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            if (_process == null)
            {
                throw new InvalidOperationException("Pipeline should be initialized with a process to use this method");
            }

            Func<CancellationToken, TInput, PipelineMetadata, Task> dataReceiver = (token, input, metadata) => Task.CompletedTask;
            if (_following != null) 
                dataReceiver = _following.Push;

            return _process(cancellationToken, dataReceiver);
        }

        public Task Push(CancellationToken cancellationToken, TInput input, PipelineMetadata metadata)
        {
            return _following?.Push(cancellationToken, input, metadata);
        }

        public Task Flush(CancellationToken cancellationToken)
        {
            return _following?.Flush(cancellationToken);
        }

        void IProducerBlock<TInput>.Link(IReceiverBlock<TInput> receiverBlock)
        {
            _following = receiverBlock;
        }
    }
}