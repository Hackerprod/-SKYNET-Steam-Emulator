using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Callback
{
    public class CallbackHandler
    {
        public static Dictionary<int, Queue<InternalCallbackMsg>> PendingCallbacks { get; set; } = new Dictionary<int, Queue<InternalCallbackMsg>>();

        public static void PostCallback(int pipe_id, int user_id, int callback_id, Buffer b)
        {
            var new_callback = new InternalCallbackMsg
            {
                user_id = user_id,
                callback_id = (uint)callback_id,
                data = b.GetBuffer(),
            };

            if (PendingCallbacks.TryGetValue(pipe_id, out var cb_queue))
            {
                cb_queue.Enqueue(new_callback);
                return;
            }

            PendingCallbacks[pipe_id] = new Queue<InternalCallbackMsg>();
            PendingCallbacks[pipe_id].Enqueue(new_callback);
        }

        // Dequeues the next callback for the pipe and return it
        public static InternalCallbackMsg GetCallbackForPipe(int pipe_id)
        {
            return PendingCallbacks[pipe_id].Count > 0 ? PendingCallbacks[pipe_id].Dequeue() : null;
        }
    }

}
