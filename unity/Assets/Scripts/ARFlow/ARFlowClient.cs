using System;
using Cysharp.Net.Http;
using Grpc.Net.Client;
using UnityEngine;

namespace ARFlow
{
    /// <summary>
    /// This class represent the implementation for the client, using the gRPC protocol generated by Protobuf.
    /// The client of ARFlow allows registering to the server and sending data frames to the server.
    /// </summary>
    public class ARFlowClient
    {
        private readonly GrpcChannel _channel;
        private readonly ARFlowService.ARFlowServiceClient _client;
        private string _sessionId;

        /// <summary>
        /// Initialize the client
        /// </summary>
        /// <param name="address">The address (AKA server URL) to connect to</param>
        public ARFlowClient(string address)
        {
            var handler = new YetAnotherHttpHandler() { Http2Only = true };
            _channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions()
            {
                HttpHandler = handler,
                MaxReceiveMessageSize = null
            });
            _client = new ARFlowService.ARFlowServiceClient(_channel);
        }

        ~ARFlowClient()
        {
            _channel.Dispose();
        }

        /// <summary>
        /// Connect to the server with a request that contain register data of about the camera.
        /// </summary>
        /// <param name="requestData">Register data (AKA metadata) of the camera. The typing of this is generated by Protobuf.</param>
        public void Connect(RegisterRequest requestData)
        {
            try
            {
                var response = _client.register(requestData);
                _sessionId = response.Uid;

                Debug.Log(response.Uid);
            }
            catch (Exception e)
            {
                // Try to catch any exceptions.
                // Network, device image, camera intrinsics
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// Send a data of a frame to the server.
        /// </summary>
        /// <param name="frameData">Data of the frame. The typing of this is generated by Protobuf.</param>
        public string SendFrame(DataFrameRequest frameData)
        {
            string res = "";
            frameData.Uid = _sessionId;
            try
            {
                // _client.data_frameAsync(frameData)
                // .ResponseAsync.ContinueWith(response =>
                // {
                //     Debug.Log(response);
                // });
                var response = _client.data_frame(frameData); ;
                res = response.Message;
            }
            catch (Exception e)
            {
                // Try to catch any exceptions.
                // Network, device image, camera intrinsics
                Debug.LogError(e);
            }

            return res;
        }
    }
}
