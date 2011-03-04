using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Hardware;
using Vixen.Execution;
using Vixen.Sequence;
using Vixen.Module.Sequence;

namespace Vixen.Sys {
    public class Execution {
        static private Dictionary<Guid, ExecutionContext> _contexts = new Dictionary<Guid, ExecutionContext>();

		// These are system-level events.
		static public event EventHandler ExecutionContextCreated;
		static public event EventHandler ExecutionContextReleased;

        static public ExecutionContext CreateContext(Program program) {
            ExecutionContext context = new ExecutionContext(program);
            _contexts[context.Id] = context;
			if(ExecutionContextCreated != null) {
				ExecutionContextCreated(context, EventArgs.Empty);
			}
            
			return context;
        }

        static public ExecutionContext CreateContext(ISequenceModuleInstance sequence) {
            Program program = new Program(sequence.Name);
            program.Add(sequence);
            return CreateContext(program);
        }

        static public void ReleaseContext(ExecutionContext context) {
            if(_contexts.ContainsKey(context.Id)) {
                _contexts.Remove(context.Id);
				if(ExecutionContextReleased != null) {
					ExecutionContextReleased(context, EventArgs.Empty);
				}
				context.Dispose();
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="contextName"></param>
		/// <returns>The resulting length of the queue.  0 if it cannot be added.</returns>
		static public int QueueSequence(ISequenceModuleInstance sequence, string contextName = null) {
			// Look for an execution context with that name.
			ExecutionContext context = _contexts.Values.FirstOrDefault(x => x.Name.Equals(contextName, StringComparison.OrdinalIgnoreCase));
			if(context != null) {
				// Context already exists.  Add sequence to it.
				// Can't just add the sequence to the program because it's executing and the
				// thing executing it has likely cached the state.  Need to go through the
				// appropriate layers.
				return context.Queue(sequence);
			} else {
				// Context does not exist.
				// The context must be created and managed since the user is not doing it.
				context = CreateContext(sequence);
				// If they explicitly specified a context name, override the existing name.
				if(!string.IsNullOrWhiteSpace(contextName)) {
					context.Program.Name = contextName;
				}
				// When the program ends, release the context.
				context.ProgramEnded += (sender, e) => {
					ReleaseContext(context);
				};
				context.Play();
				// It is the sequence playing now.
				return 0;
			}
		}
    }
}
