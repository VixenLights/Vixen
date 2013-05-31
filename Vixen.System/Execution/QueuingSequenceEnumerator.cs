using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.Execution
{
    class QueuingSequenceEnumerator : IEnumerator<ISequenceExecutor>
    {
        private ISequence[] _sequences;
        private Queue<ISequence> _sequenceQueue;

        public QueuingSequenceEnumerator(IEnumerable<ISequence> sequences)
        {
            _sequences = sequences.ToArray();
            Reset();
        }

        public ISequenceExecutor Current { get; private set; }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if (Current != null)
            {
                // Cleanup after the prior sequence.
                _DisposeCurrent();
            }

            // Anything left to play?
            if (_sequenceQueue.Count > 0)
            {
                // Get the sequence.
                ISequence sequence = _sequenceQueue.Dequeue();
                // Get an executor for the sequence.
                Current = SequenceTypeService.Instance.CreateSequenceExecutor(sequence);
                if (Current == null) throw new InvalidOperationException(string.Format("Sequence {0}  has no executor.", sequence.Name));

                Current.Sequence = sequence;

                return true;
            }
            return false;
        }

        public void Reset()
        {
            _DisposeCurrent();
            _sequenceQueue = new Queue<ISequence>(_sequences);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns>The resulting length of the queue.  0 if it cannot be added.</returns>
        public int Queue(ISequence sequence)
        {
            if (_sequenceQueue != null)
            {
                _sequenceQueue.Enqueue(sequence);
                return _sequenceQueue.Count;
            }
            return 0;
        }

        public void Dispose()
        {
            _DisposeCurrent();
        }

        private void _DisposeCurrent()
        {
            if (Current != null)
            {
                Current.Dispose();
                Current = null;
            }
        }
    }
}
