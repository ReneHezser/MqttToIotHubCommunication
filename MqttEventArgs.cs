using System;

namespace MqttToIotHub
{
    class MqttEventArgs : EventArgs
    {
        public MqttEventArgs()
        {
        }

        public MqttEventArgs(string topic, object value) : this()
        {
            Message = topic;
            Value = value;
        }

        public string Message { get; set; }
        public object Value { get; set; }
    }
}