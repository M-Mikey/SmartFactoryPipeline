using Azure.Messaging.ServiceBus;
using MachineData;
using MachineData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineWorker
{
    public class CircuitBreaker
    {
        // just 4 fields — no logger, no DB, no Service Bus
        private int _failureCount;
        private DateTime _lastFailureTime;
        private readonly int _threshold;
        private readonly TimeSpan _cooldown;

        public CircuitBreaker(int threshold, TimeSpan cooldown)
        {
            _threshold = threshold;
            _cooldown = cooldown;
        }

        public bool IsOpen()
        {

            
            return _failureCount >= _threshold && (DateTime.UtcNow - _lastFailureTime) < _cooldown;
        }

        public void RecordFailure() 
        { 
            _lastFailureTime = DateTime.UtcNow;
            _failureCount++;
        }
        public void RecordSuccess() 
        {
            _failureCount = 0;
            _lastFailureTime = default;
        }
    }
}
