using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Connection.Test
{
    public partial class TestConnection
    {
        public class HostFake : IDisposable
        {
            private bool Disposing = false;
            private bool WriteToRxBufferRunning = false;
            private int InputRxBufferDelay = 0;
            private byte[] InputRxBuffer = new byte[0];
            private byte[] RxBuffer = new byte[0];
            private byte[] TxBuffer = new byte[0];

            public HostFake()
            {
                WriteToRxBuffer();
            }

            public async Task<bool> ConnectAsync(int delay, bool result)
            {
                await Task.Delay(delay);
                return result;
            }

            public async Task<bool> DataAvaliable()
            {
                return await Task.Run(() =>
                {
                    lock (RxBuffer)
                    {
                        return RxBuffer.Length > 0;

                    }
                });
            }

            public async Task<bool> DisconnectAsync(int delay, bool result)
            {
                await Task.Delay(delay);
                return result;
            }

            public void Dispose()
            {
                Disposing = true;
                Task.Run(() =>
                {
                    while (WriteToRxBufferRunning)
                    {
                        if (!WriteToRxBufferRunning) break;
                    }

                }).Wait();
            }

            public async Task<int> ReadAsync(byte[] data, CancellationToken cancellationToken)
            {
                return await Task.Run(() =>
                {
                    int bytesRead = 0;
                    lock (RxBuffer)
                    {
                        if (RxBuffer?.Length > 0)
                        {
                            bytesRead = RxBuffer.Length;
                            RxBuffer.CopyTo(data, 0);
                            Array.Resize(ref RxBuffer, 0);
                        }
                    }
                    if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();
                    return bytesRead;
                });
            }

            public async Task WriteToInputRxBuffer(byte[] data, int delayPerByte = 0)
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        lock (InputRxBuffer)
                        {
                            if (InputRxBuffer.Length == 0)
                            {
                                InputRxBufferDelay = delayPerByte;
                                Array.Resize(ref InputRxBuffer, data.Length);
                                data.CopyTo(InputRxBuffer, 0);
                                break;
                            }
                        }
                    }
                });
            }

            private void WriteToRxBuffer()
            {
                WriteToRxBufferRunning = true;
                try
                {
                    Task.Run(async () =>
                    {
                        while (!Disposing)
                        {
                            if (InputRxBuffer.Length > 0)
                            {
                                for (int i = 0; i < InputRxBuffer.Length; i++)
                                {
                                    await Task.Delay(InputRxBufferDelay);
                                    lock (InputRxBuffer)
                                    {
                                        lock (RxBuffer)
                                        {
                                            Array.Resize(ref RxBuffer, RxBuffer.Length + 1);
                                            Array.Copy(InputRxBuffer, i, RxBuffer, RxBuffer.Length - 1, 1);
                                        }
                                    }
                                }
                                Array.Resize(ref InputRxBuffer, 0);
                            }
                        }
                    });
                }
                finally
                {
                    WriteToRxBufferRunning = false;
                }
            }

            public async Task<bool> WriteAsync(byte[] data, CancellationToken cancellationToken)
            {
                return await Task.Run(() =>
                {
                    Array.Resize(ref TxBuffer, data.Length);
                    data.CopyTo(TxBuffer, 0);
                    if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();
                    return true;
                });
            }


        }
    }
}
