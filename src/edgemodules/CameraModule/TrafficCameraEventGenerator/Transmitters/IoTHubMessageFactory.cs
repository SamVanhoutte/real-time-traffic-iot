using Microsoft.Azure.Devices.Client;

using System;
using System.Collections.Generic;
using System.Text;
using TrafficCameraEventGenerator.Cars;

namespace TrafficCameraEventGenerator.Transmitters
{
    public static class IoTHubMessageFactory
    {
        public static Message CreateMessage(CameraEvent cameraEvent)
        {
            var msg = new Message(Encoding.UTF8.GetBytes(cameraEvent.ToJson()));
            msg.ContentEncoding = "UTF-8";
            msg.ContentType = "application/json";
            return msg;
        }
    }
}
