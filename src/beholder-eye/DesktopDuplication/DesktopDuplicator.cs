namespace beholder_eye
{
    using Microsoft.Extensions.Logging;
    using SharpGen.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Vortice;
    using Vortice.Direct3D;
    using Vortice.Direct3D11;
    using Vortice.DXGI;
    using MapFlags = Vortice.Direct3D11.MapFlags;
    using Point = Vortice.Mathematics.Point;
    using Rectangle = System.Drawing.Rectangle;
    using Usage = Vortice.Direct3D11.Usage;

    /// <summary>
    /// Provides access to frame-by-frame updates of a particular desktop (i.e. one monitor), with image and cursor information.
    /// </summary>
    public sealed class DesktopDuplicator
    {
        private readonly int _adapterIndex = -1;
        private readonly int _outputDeviceIndex = -1;
        private readonly ILogger _logger;

        /// <summary>
        /// Duplicates the output of the specified monitor on the specified graphics adapter.
        /// </summary>
        /// <param name="logger">The ILogger to log error messages.</param>
        /// <param name="adapterIndex">The index of the graphics card adapter which contains the desired outputs.</param>
        /// <param name="outputDeviceIndex">The index of the output device to duplicate (i.e. monitor). Begins with zero, which seems to correspond to the primary monitor.</param>
        public DesktopDuplicator(ILogger logger, int adapterIndex = 0, int outputDeviceIndex = 0)
        {
            if (adapterIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(adapterIndex));
            }

            if (outputDeviceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(outputDeviceIndex));
            }

            _adapterIndex = adapterIndex;
            _outputDeviceIndex = outputDeviceIndex;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Duplicates the desktop yielding Desktop Frames until cancelled.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<DesktopFrame> DuplicateDesktop(CancellationToken token)
        {
            _logger.LogInformation($"Beginning Desktop Duplication");
            using var duplicator = DesktopDuplicatorInternal.CreateDesktopDuplicator(_logger, _adapterIndex, _outputDeviceIndex);

            while (token.IsCancellationRequested == false)
            {
                var desktopFrame = new DesktopFrame();
                UpdateFrame(duplicator, desktopFrame);

                if (token.IsCancellationRequested == false)
                {
                    if (desktopFrame.IsDesktopImageBufferEmpty)
                    {
                        continue;
                    }

                    yield return desktopFrame;
                }
            }
        }

        /// <summary>
        /// Updates the specified frame with the next retrieved frame.
        /// </summary>
        private void UpdateFrame(DesktopDuplicatorInternal ddupe, DesktopFrame frame)
        {
            // Try to get the latest frame;
            if (ddupe.TryRetrieveFrame(out var frameInfo))
            {
                try
                {
                    ddupe.RetrieveFrameMetadata(frame, frameInfo);
                    ddupe.RetrieveCursorMetadata(frame, frameInfo);
                    ddupe.ProcessFrame(frame);
                }
                finally
                {
                    ddupe.ReleaseFrame(_logger);
                }
            }
        }

        /// <summary>
        /// Represents the internal implementation of a desktop duplicator.
        /// </summary>
        private sealed class DesktopDuplicatorInternal : IDisposable
        {
            private ID3D11Device _d3dDevice;
            private ID3D11DeviceContext _immediateContext;
            private RawRect _desktopRect;

            private ID3D11Texture2D _desktopImageTexture;
            private IDXGIOutputDuplication _outputDuplication;

            private int _outputDeviceIndex;
            private readonly PointerInfo _pointerInfo = new PointerInfo();

            private DesktopDuplicatorInternal()
            {
            }

            public bool IsDisposed
            {
                get;
                private set;
            }

            /// <summary>
            /// Retrieves the next frame and copies the texture into the _desktopImageTexture.
            /// </summary>
            /// <param name="frameInformation"></param>
            /// <returns></returns>
            public bool TryRetrieveFrame(out OutduplFrameInfo frameInformation)
            {
                IDXGIResource desktopResource = null;
                try
                {
                    _outputDuplication.AcquireNextFrame(100, out frameInformation, out desktopResource);
                    using var tempTexture = desktopResource.QueryInterface<ID3D11Texture2D>();
                    _d3dDevice.ImmediateContext.CopyResource(_desktopImageTexture, tempTexture);
                    return true;
                }
                catch (SharpGenException ex)
                {
                    if (ex.ResultCode.Failure && ex.Descriptor.NativeApiCode != "DXGI_ERROR_ACCESS_LOST" && ex.Descriptor.NativeApiCode != "DXGI_ERROR_WAIT_TIMEOUT")
                    {
                        throw new DesktopDuplicationException("Failed to acquire next frame.");
                    }
                }
                finally
                {
                    if (desktopResource != null)
                    {
                        desktopResource.Dispose();
                    }
                }

                frameInformation = new OutduplFrameInfo();
                return false;
            }

            /// <summary>
            /// Retrieves the moved and dirty regions of the currently duplicated frame, populating the specified desktop frame.
            /// </summary>
            /// <param name="frame"></param>
            /// <param name="frameInfo"></param>
            public void RetrieveFrameMetadata(DesktopFrame frame, OutduplFrameInfo frameInfo)
            {
                if (frameInfo.TotalMetadataBufferSize > 0)
                {
                    var movedRectangles = new OutduplMoveRect[frameInfo.TotalMetadataBufferSize];

                    // Get moved regions
                    _outputDuplication.GetFrameMoveRects(movedRectangles.Length, movedRectangles, out int movedRegionsLength);
                    var movedRegions = new MovedRegion[movedRegionsLength / Marshal.SizeOf(typeof(OutduplMoveRect))];
                    for (int i = 0; i < movedRegions.Length; i++)
                    {
                        movedRegions[i] = new MovedRegion()
                        {
                            Source = new Point(movedRectangles[i].SourcePoint.X, movedRectangles[i].SourcePoint.Y),
                            Destination = new Rectangle(movedRectangles[i].DestinationRect.Left, movedRectangles[i].DestinationRect.Top, movedRectangles[i].DestinationRect.Right, movedRectangles[i].DestinationRect.Bottom)
                        };
                    }
                    frame.MovedRegions = movedRegions;

                    // Get dirty regions
                    var dirtyRectangles = new RawRect[frameInfo.TotalMetadataBufferSize];
                    _outputDuplication.GetFrameDirtyRects(dirtyRectangles.Length, dirtyRectangles, out int dirtyRegionsLength);
                    var updatedRegions = new Rectangle[dirtyRegionsLength / Marshal.SizeOf(typeof(Rectangle))];
                    for (int i = 0; i < updatedRegions.Length; i++)
                    {
                        updatedRegions[i] = new Rectangle(dirtyRectangles[i].Left, dirtyRectangles[i].Top, dirtyRectangles[i].Right, dirtyRectangles[i].Bottom);
                    }
                    frame.UpdatedRegions = updatedRegions;
                }
                else
                {
                    frame.MovedRegions = Array.Empty<MovedRegion>();
                    frame.UpdatedRegions = Array.Empty<Rectangle>();
                }
            }

            /// <summary>
            /// Retrieves the cursor information contained in the currently duplicated frame, populating the specified desktop frame.
            /// </summary>
            /// <param name="frame"></param>
            /// <param name="frameInfo"></param>
            public void RetrieveCursorMetadata(DesktopFrame frame, OutduplFrameInfo frameInfo)
            {
                // A non-zero mouse update timestamp indicates that there is a mouse position update and optionally a shape change
                if (frameInfo.LastMouseUpdateTime == 0)
                {
                    frame.PointerPosition.Visible = _pointerInfo.Visible;
                    frame.PointerPosition.X = _pointerInfo.Position.X;
                    frame.PointerPosition.Y = _pointerInfo.Position.Y;
                    return;
                }

                bool updatePosition = true;

                // Make sure we don't update pointer position wrongly
                // If pointer is invisible, make sure we did not get an update from another output that the last time that said pointer
                // was visible, if so, don't set it to invisible or update.

                if (!frameInfo.PointerPosition.Visible && (_pointerInfo.WhoUpdatedPositionLast != _outputDeviceIndex))
                    updatePosition = false;

                // If two outputs both say they have a visible, only update if new update has newer timestamp
                if (frameInfo.PointerPosition.Visible && _pointerInfo.Visible &&
                    (_pointerInfo.WhoUpdatedPositionLast != _outputDeviceIndex) &&
                    (_pointerInfo.LastTimeStamp > frameInfo.LastMouseUpdateTime))
                    updatePosition = false;

                // Update position
                if (updatePosition)
                {
                    _pointerInfo.Position = new Point(frameInfo.PointerPosition.Position.X, frameInfo.PointerPosition.Position.Y);
                    _pointerInfo.WhoUpdatedPositionLast = _outputDeviceIndex;
                    _pointerInfo.LastTimeStamp = frameInfo.LastMouseUpdateTime;
                    _pointerInfo.Visible = frameInfo.PointerPosition.Visible;
                }

                frame.PointerPosition.Visible = _pointerInfo.Visible;
                frame.PointerPosition.X = _pointerInfo.Position.X;
                frame.PointerPosition.Y = _pointerInfo.Position.Y;

                // No new shape
                if (frameInfo.PointerShapeBufferSize == 0)
                    return;

                if (_pointerInfo.ShapeBuffer == null || frameInfo.PointerShapeBufferSize != _pointerInfo.ShapeBuffer.Length)
                {
                    _pointerInfo.ShapeBuffer = new byte[frameInfo.PointerShapeBufferSize];
                }

                try
                {
                    // Create a new pinned region of memory, copy the contents of the PtrShapeBuffer to it, use it to retrieve the pointer shape and then free the pinned region.
                    unsafe
                    {
                        fixed (byte* ptrShapeBufferPtr = _pointerInfo.ShapeBuffer)
                        {
                            _outputDuplication.GetFramePointerShape(frameInfo.PointerShapeBufferSize, (IntPtr)ptrShapeBufferPtr, out int bufferSize, out _pointerInfo.ShapeInfo);
                        }
                    }
                }
                catch (SharpGenException ex)
                {
                    if (ex.ResultCode.Failure)
                    {
                        throw new DesktopDuplicationException("Failed to get frame pointer shape.");
                    }
                }

                var pointerShapeSpan = new ReadOnlySpan<byte>(_pointerInfo.ShapeBuffer);
                if (!pointerShapeSpan.Trim((byte)0x00).IsEmpty)
                {
                    frame.PointerShape.Width = _pointerInfo.ShapeInfo.Width;
                    frame.PointerShape.Height = _pointerInfo.ShapeInfo.Height;
                    frame.PointerShape.Pitch = _pointerInfo.ShapeInfo.Pitch;
                    frame.PointerShape.Type = _pointerInfo.ShapeInfo.Type;
                    frame.PointerShape.HotSpotX = _pointerInfo.ShapeInfo.HotSpot.X;
                    frame.PointerShape.HotSpotY = _pointerInfo.ShapeInfo.HotSpot.Y;
                    frame.PointerShapeBuffer = _pointerInfo.ShapeBuffer;
                }
            }

            /// <summary>
            /// Using the staging texture
            /// </summary>
            /// <param name="frame"></param>
            public void ProcessFrame(DesktopFrame frame)
            {
                frame.DesktopWidth = Math.Abs(_desktopRect.Right - _desktopRect.Left);
                frame.DesktopHeight = Math.Abs(_desktopRect.Bottom - _desktopRect.Top);

                // Get the desktop capture texture
                var mapSource = _d3dDevice.ImmediateContext.Map(_desktopImageTexture, 0, MapMode.Read, MapFlags.None);
                var buffer = new byte[frame.DesktopHeight * mapSource.RowPitch];

                // Copy pixels from screen capture Texture to the image buffer.
                var bufferSpan = new Span<byte>(buffer);
                mapSource.AsSpan<byte>(buffer.Length).CopyTo(bufferSpan);
                
                // Release source and dest locks
                _d3dDevice.ImmediateContext.Unmap(_desktopImageTexture, 0);

                frame.DesktopFrameBuffer = buffer;
                frame.IsDesktopImageBufferEmpty = bufferSpan.Trim((byte)0x00).IsEmpty;
            }

            public void ReleaseFrame(ILogger logger)
            {
                try
                {
                    _outputDuplication.ReleaseFrame();
                }
                catch (SharpGenException ex)
                {
                    if (ex.ResultCode.Failure)
                    {
                        logger.LogWarning($"Failed to release frame: {ex.Descriptor.Description}");
                    }
                }
            }

            #region IDisposable Support
            private void Dispose(bool disposing)
            {
                if (!IsDisposed)
                {
                    if (disposing)
                    {

                        if (!_desktopImageTexture.IsDisposed)
                        {
                            _desktopImageTexture.Dispose();
                        }

                        if (_outputDuplication != null && !_outputDuplication.IsDisposed)
                        {
                            try
                            {
                                _outputDuplication.ReleaseFrame();
                            }
                            catch
                            {
                                // Do Nothing...
                            }

                            _outputDuplication.Dispose();
                        }

                        if (_immediateContext != null && !_immediateContext.IsDisposed)
                        {
                            _immediateContext.Dispose();
                        }

                        if (_d3dDevice != null && !_d3dDevice.IsDisposed)
                        {
                            _d3dDevice.Dispose();
                        }
                    }
                    IsDisposed = true;
                }
            }

            public void Dispose()
            {
                Dispose(true);
            }

            #endregion

            public static DesktopDuplicatorInternal CreateDesktopDuplicator(ILogger logger, int adapterIndex, int outputDeviceIndex)
            {
                var dd = new DesktopDuplicatorInternal
                {
                    _outputDeviceIndex = outputDeviceIndex
                };

                var createFactoryResult = DXGI.CreateDXGIFactory1(out IDXGIFactory1 factory);
                if (!createFactoryResult.Success)
                {
                    throw new DesktopDuplicationException("Couldn't create a DXGI Factory.");
                }

                IDXGIAdapter1 adapter = null;
                IDXGIOutput output = null;
                try
                {
                    var result = factory.EnumAdapters1(adapterIndex, out adapter);
                    if (result.Failure)
                    {
                        throw new DesktopDuplicationException($"An error occurred attempting to retrieve the adapter at the specified index ({adapterIndex}): {result}");
                    }

                    if (adapter == null)
                    {
                        throw new DesktopDuplicationException($"An adapter was not found at the specified index ({adapterIndex}).");
                    }

                    logger.LogInformation($"Using adapter at index {adapterIndex} - {adapter.Description.Description}");

                    var createD3dDeviceResult = D3D11.D3D11CreateDevice(adapter, DriverType.Unknown, DeviceCreationFlags.None, null, out dd._d3dDevice, out dd._immediateContext);
                    if (!createD3dDeviceResult.Success)
                    {
                        throw new DesktopDuplicationException("Couldn't create a D3D device from the specified adapter.");
                    }

                    using var device = dd._d3dDevice.QueryInterface<IDXGIDevice>();
                    var outputResult = adapter.EnumOutputs(outputDeviceIndex, out output);
                    if (outputResult.Failure)
                    {
                        throw new DesktopDuplicationException($"An error occurred attempting to retrieve the output device at the specified index ({outputDeviceIndex}): {outputResult}");
                    }

                    if (output == null)
                    {
                        throw new DesktopDuplicationException($"An output was not found at the specified index ({outputDeviceIndex}).");
                    }

                    logger.LogInformation($"Using output device on adapter {adapterIndex} at index {outputDeviceIndex}.");

                    var output6 = output.QueryInterface<IDXGIOutput6>();
                    try
                    {
                        // Copy the values to a new rect.
                        var rectTemp = output6.Description.DesktopCoordinates;
                        dd._desktopRect = new RawRect(rectTemp.Left, rectTemp.Top, rectTemp.Right, rectTemp.Bottom);

                        dd._outputDuplication = output6.DuplicateOutput(device);

                        var stagingTexture = new Texture2DDescription()
                        {
                            CpuAccessFlags = CpuAccessFlags.Read,
                            BindFlags = BindFlags.None,
                            Format = dd._outputDuplication.Description.ModeDescription.Format,
                            Width = Math.Abs(dd._desktopRect.Right - dd._desktopRect.Left),
                            Height = Math.Abs(dd._desktopRect.Bottom - dd._desktopRect.Top),
                            OptionFlags = ResourceOptionFlags.None,
                            MipLevels = 1,
                            ArraySize = 1,
                            SampleDescription = { Count = 1, Quality = 0 },
                            Usage = Usage.Staging // << can be read by CPU
                        };

                        // Initialize the Output Duplication -- If this isn't done occassionally an 'Unsupported' result will occur with DuplicationOutput1
                        dd._desktopImageTexture = dd._d3dDevice.CreateTexture2D(stagingTexture);
                    }
                    catch (SharpGenException ex)
                    {
                        if (ex.Descriptor.NativeApiCode == "DXGI_ERROR_UNSUPPORTED")
                        {
                            throw new DesktopDuplicationException("Unsupported desktop mode or scenario.");
                        }
                        else if (ex.Descriptor.NativeApiCode == "DXGI_ERROR_NOT_CURRENTLY_AVAILABLE")
                        {
                            throw new DesktopDuplicationException("There is already the maximum number of applications using the Desktop Duplication API running, please close one of the applications and try again.");
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                finally
                {
                    if (output != null)
                    {
                        output.Dispose();
                    }

                    if (adapter != null)
                    {
                        adapter.Dispose();
                    }

                    if (factory != null)
                    {
                        factory.Dispose();
                    }
                }

                return dd;
            }
        }
    }
}
