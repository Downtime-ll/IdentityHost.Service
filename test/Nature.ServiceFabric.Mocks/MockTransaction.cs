﻿using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;

namespace Nature.ServiceFabric.Mocks
{
    public class MockTransaction : ITransaction
    {
        public Task CommitAsync()
        {
            return Task.FromResult(true);
        }

        public void Abort()
        {
        }

        public long TransactionId => 0L;

        public long CommitSequenceNumber
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
        }

        public Task<long> GetVisibilitySequenceNumberAsync()
        {
            return Task.FromResult(0L);
        }
    }
}
