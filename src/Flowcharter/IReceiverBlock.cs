﻿using System.Threading;

namespace Flowcharter
{
    public interface IReceiverBlock<in TInput>
    {
        void Push(TInput input, PipelineMetadata metadata);
        void Flush();
    }
}