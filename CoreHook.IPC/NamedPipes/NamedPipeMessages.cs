﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoreHook.IPC.NamedPipes
{
    public static partial class NamedPipeMessages
    {
        private const string ResponseSuffix = "Response";
        public const string UnknownRequest = "UnknownRequest";
        private const char MessageSeparator = '|';

        public enum CompletionState
        {
            NotCompleted,
            Success,
            Failure
        }

        public class BaseResponse<TRequest>
        {
            public const string Header = nameof(TRequest) + ResponseSuffix;
            public CompletionState State { get; set; }
            public string ErrorMessage { get; set; }
            public Message ToMessage()
            {
                return new Message(Header, JsonConvert.SerializeObject(this));
            }
        }

        public class Message
        {
            public Message(string header, string body)
            {
                Header = header;
                Body = body;
            }
            public string Header { get; }
            public string Body { get; }
            public static Message FromString(string message)
            {
                string header = null;
                string body = null;
                if (!string.IsNullOrEmpty(message))
                {
                    string[] parts = message.Split(new[] { NamedPipeMessages.MessageSeparator }, count: 2);
                    header = parts[0];
                    if (parts.Length > 1)
                    {
                        body = parts[1];
                    }
                }
                return new Message(header, body);
            }
            public override string ToString()
            {
                string result = string.Empty;
                if (!string.IsNullOrEmpty(this.Header))
                {
                    result = this.Header;
                }
                if (this.Body != null)
                {
                    result = result + NamedPipeMessages.MessageSeparator + this.Body;
                }
                return result;
            }
        }

        public static class Notification
        {
            public class Request
            {
                public const string Header = nameof(Notification);
                public int ProcessId { get; set; }
                public string Message { get; set; }
                public static Request FromMessage(Message message)
                {
                    return JsonConvert.DeserializeObject<Request>(message.Body);
                }
                public Message ToMessage()
                {
                    return new Message(Header, JsonConvert.SerializeObject(this));
                }
            }
        }
    }
}
