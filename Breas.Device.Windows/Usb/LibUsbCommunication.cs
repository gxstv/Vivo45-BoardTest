using Breas.Device.Communication;
using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Breas.Device.Finder.Windows.Usb
{
    public class LibUsbCommunication : IStreamCommunication
    {
        private const int EndPointLength = 10000;

        private UsbRegistry _usbRegistry;
        private UsbDevice _usbDevice;
        private UsbResolverInfo _resolverInfo;

        // EP2: Bulk OUT - used to send BCP commands/packets to the unit.
        private UsbEndpointWriter _endpointWriter;
        // EP6: Bulk IN - used to send BCP replies to the USB host.
        private UsbEndpointReader _endpointReader;
        // EP8: Bulk IN - used to send “real time capture data” to the host.
        private UsbEndpointReader _realTimeEndpointReader;
        private bool resetWhenDisconnected;

        public IResolverInfo ResolverInfo
        {
            get { return _resolverInfo; }
        }

        public bool FailedInit { get; set; }

        public bool Heartbeat
        {
            get { return _usbDevice != null && _usbDevice.IsOpen; }
        }

        public bool StayConnected
        {
            get
            {
                return true;
            }
        }

        public LibUsbCommunication(UsbRegistry usbRegistry, UsbResolverInfo resolverInfo)
        {
            _usbRegistry = usbRegistry;
            _resolverInfo = resolverInfo;
        }

        public bool Connect()
        {
            _usbRegistry.Open(out _usbDevice);
            if (_usbDevice == null)
            {
                return false;
            }

            // If this is a "whole" usb device (libusb-win32, linux libusb)
            // it will have an IUsbDevice interface. If not (WinUSB) the 
            // variable will be null indicating this is an interface of a 
            // device.
            IUsbDevice wholeUsbDevice = _usbDevice as IUsbDevice;
            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                // This is a "whole" USB device. Before it can be used, 
                // the desired configuration and interface must be selected.

                // Select config #1
                wholeUsbDevice.SetConfiguration(1);

                // Claim interface #0.
                if (!wholeUsbDevice.ClaimInterface(0))
                {
                    wholeUsbDevice.ReleaseInterface(0);
                    _usbDevice.Close();
                    return false;
                }
            }
            if (_resolverInfo.V45Endpoints == true)
            {
                // Open EP1 for writing, used to send BCP commands/packets to the unit.
                _endpointWriter = _usbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

                // Open EP2 for reading, used to send BCP replies to the USB host.
                _endpointReader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep02);

                // Open EP5 for reading, used to send “real time capture data” to the host.
                _realTimeEndpointReader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep05);
            }
            else
            { 
            // Open EP2 for writing, used to send BCP commands/packets to the unit.
            _endpointWriter = _usbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);

            // Open EP6 for reading, used to send BCP replies to the USB host.
            _endpointReader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep06);

            // Open EP8 for reading, used to send “real time capture data” to the host.
            _realTimeEndpointReader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep08);
            }
            return true;
        }

        public bool Disconnect()
        {
            if (_usbDevice != null)
            {
                if (_usbDevice.IsOpen)
                {
                    _endpointWriter.Flush();
                    _endpointReader.Flush();
                    _realTimeEndpointReader.Flush();

                    // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                    // it exposes an IUsbDevice interface. If not (WinUSB) the 
                    // 'wholeUsbDevice' variable will be null indicating this is 
                    // an interface of a device; it does not require or support 
                    // configuration and interface selection.
                    IUsbDevice wholeUsbDevice = _usbDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Release interface #0.
                        wholeUsbDevice.ReleaseInterface(0);
                    }
                    
                    if (FailedInit || resetWhenDisconnected)
                    {
                        if (_usbDevice is LibUsbDevice)
                        {
                            var libUsbDevice = (LibUsbDevice)_usbDevice;
                            //coming out of sleep the device needs to be reset. or sometimes the vivo gets weird so its best to reset
                            libUsbDevice.ResetDevice();
                        }
                    }
                    _usbDevice.Close();
                }

                _usbDevice = null;
            }

            return true;
        }

        public byte[] GetMessage(int timeout = 1000)
        {
            byte[] data = null;
            UsbTransfer usbTransfer = null;

            try
            {
                int bytesRead;
                byte[] readBuffer = new byte[EndPointLength];

                ErrorCode errorCode = _endpointReader.SubmitAsyncTransfer(readBuffer, 0, readBuffer.Length, timeout, out usbTransfer);
                if (errorCode != ErrorCode.None)
                {
                    //throw new Exception(UsbDevice.LastErrorString);
                    return null;
                }

                errorCode = usbTransfer.Wait(out bytesRead);
                if (errorCode != ErrorCode.None)
                {
                    if (errorCode == ErrorCode.IoTimedOut)
                    {
                        resetWhenDisconnected = true;
                    }
                    //throw new Exception(UsbDevice.LastErrorString);
                    return null;
                }

                data = new byte[bytesRead];
                Array.Copy(readBuffer, data, bytesRead);
            }
            finally
            {
                if (usbTransfer != null)
                {
                    if (!usbTransfer.IsCancelled || !usbTransfer.IsCompleted)
                    {
                        usbTransfer.Cancel();
                    }

                    usbTransfer.Dispose();
                }
            }

            return data;
        }

        public byte[] SendMessage(byte[] message, int timeout = 1000)
        {
            UsbTransfer usbTransfer = null;

            try
            {
                int bytesWritten;

                ErrorCode errorCode = _endpointWriter.SubmitAsyncTransfer(message, 0, message.Length, timeout, out usbTransfer);
                if (errorCode != ErrorCode.None)
                {
                    //throw new Exception(UsbDevice.LastErrorString);
                    return null;
                }

                errorCode = usbTransfer.Wait(out bytesWritten);
                if (errorCode != ErrorCode.None)
                {
                    if (errorCode == ErrorCode.IoTimedOut)
                    {
                        resetWhenDisconnected = true;
                    }
                    //throw new Exception(UsbDevice.LastErrorString);
                    return null;
                }
            }
            finally
            {
                if (usbTransfer != null)
                {
                    if (!usbTransfer.IsCancelled || !usbTransfer.IsCompleted)
                    {
                        usbTransfer.Cancel();
                    }

                    usbTransfer.Dispose();
                }
            }

            return GetMessage(timeout);
        }

        public byte[] GetMessageStreamComm(int timeout = 10000)
        {
            byte[] data = null;
            UsbTransfer usbTransfer = null;

            try
            {
                int bytesRead;
                byte[] readBuffer = new byte[EndPointLength];

                ErrorCode errorCode = _realTimeEndpointReader.SubmitAsyncTransfer(readBuffer, 0, readBuffer.Length, timeout, out usbTransfer);
                if (errorCode != ErrorCode.None)
                {
                    //throw new Exception(UsbDevice.LastErrorString);
                    return null;
                }

                errorCode = usbTransfer.Wait(out bytesRead);
                if (errorCode != ErrorCode.None)
                {
                    if (errorCode == ErrorCode.IoTimedOut)
                    {
                        resetWhenDisconnected = true;
                    }
                    return null;
                }

                data = new byte[bytesRead];
                Array.Copy(readBuffer, data, bytesRead);
            }
            finally
            {
                if (usbTransfer != null)
                {
                    if (!usbTransfer.IsCancelled || !usbTransfer.IsCompleted)
                    {
                        usbTransfer.Cancel();
                    }

                    usbTransfer.Dispose();
                }
            }

            return data;
        }

        public Task<byte[]> GetMessageAsync(int timeout = 1000)
        {
            return Task.Run(() => GetMessage(timeout));
        }

        public Task<byte[]> SendMessageAsync(byte[] message, int timeout = 1000)
        {
            return Task.Run(() => SendMessage(message, timeout));
        }

        public Task<byte[]> GetMessageStreamCommAsync(int timeout = 1000)
        {
            return Task.Run(() => GetMessageStreamComm(timeout));
        }
    }
}
